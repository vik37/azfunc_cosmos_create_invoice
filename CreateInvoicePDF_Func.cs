using azfunc_cosmos_create_invoice.Documents;
using azfunc_cosmos_create_invoice.Entity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using System.Diagnostics;
using Container = Microsoft.Azure.Cosmos.Container;

namespace azfunc_cosmos_create_invoice;

public class CreateInvoicePDF_Func
{
	private readonly ILogger<CreateInvoicePDF_Func> _logger;
	private readonly BlobServiceClient _blobServiceClient;
	private readonly CosmosClient _cosmos;

	public CreateInvoicePDF_Func(ILogger<CreateInvoicePDF_Func> logger, BlobServiceClient blobService,
		CosmosClient cosmos)
	{
		_logger = logger;
		_blobServiceClient = blobService;
		_cosmos = cosmos;
	}

	[Function("CreateInvoicePDF_Func")]
	public async Task Run([CosmosDBTrigger(
		databaseName: "orchwaredb",
		containerName: "order",
		Connection = "CosmosConnectionString",
		LeaseContainerName = "leases",
		CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Order> orders)
	{
		var watch = new Stopwatch();
		watch.Start();
		if (orders == null || orders.Count == 0)
			return;

		try
		{
			var container = _cosmos
				.GetDatabase("orchwaredb")
				.GetContainer("order");

			var blobContainer =
				_blobServiceClient.GetBlobContainerClient("orchware");

			await blobContainer.CreateIfNotExistsAsync();

			foreach (var order in orders)
			{
				if (order.Invoice is not null && order.Invoice.PdfGenerated)
					continue;

				await CreateOrderInvoiceFile(order, blobContainer, container);

				_logger.LogInformation(
					"Invoice PDF generated for order {OrderNumber}",
					order.OrderNumber);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Creating Order Invoice Failed: {Message}", ex.Message);
		}

		watch.Stop();
		_logger.LogInformation("The entire operation to create and save the PDF invoice takes {MiliSeconds} seconds.", watch.ElapsedMilliseconds);
	}

	private async Task CreateOrderInvoiceFile(Order order, BlobContainerClient? blobContainer, Container container)
	{
		string folder = order.PaymentInfo.WillBePaidInFull
				? "receipts"
				: $"invoices";

		string fileName =
			$"invoice_{order.OrderNumber.Replace("#", "")}_{(order.PaymentInfo.WillBePaidInFull ? order.PaymentInfo.Installments?.InstallmentNumber ?? 01 : "")}.pdf";

		var blobClient =
			blobContainer.GetBlobClient($"{folder}/{fileName}");

		//Generate PDF
		await using (var stream =
			await blobClient.OpenWriteAsync(overwrite: true))
		{
			var document = new InvoiceDocument(order);
			document.GeneratePdf(stream);
		}

		var sasBuilder = new BlobSasBuilder
		{
			BlobContainerName = blobContainer.Name,
			BlobName = blobClient.Name,
			Resource = "b",
			ExpiresOn = DateTimeOffset.UtcNow.AddDays(7)
		};

		sasBuilder.SetPermissions(BlobSasPermissions.Read);

		Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

		string blobSasUrl = sasUri.ToString();

		await container.PatchItemAsync<Order>(
			id: order.Id,
			partitionKey: new PartitionKey(order.OrderNumber),
			patchOperations: new[]
			{
					PatchOperation.Set("/invoice", new
					{
						pdfGenerated = true,
						pdfUrl = blobSasUrl,
						generatedAt = DateTime.UtcNow
					})
			});
	}
}