using azfunc_cosmos_create_invoice.Entity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using static QuestPDF.Helpers.Colors;

namespace azfunc_cosmos_create_invoice.Documents;

public class InvoiceDocument : IDocument
{
	private readonly Order _order;

	public InvoiceDocument(Order order)
	{
		_order = order;
	}

	public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

	public void Compose(IDocumentContainer container)
	{
		Installment? installment = null;
		if (!_order.PaymentInfo.WillBePaidInFull)
		{
			installment = _order.PaymentInfo.Installments;
		}
		container.Page(page =>
		{
			page.Size(PageSizes.A4);
			page.DefaultTextStyle(x => x.FontSize(12));
			page.PageColor(Colors.White);

			page.Header()

			.BackgroundLinearGradient(50, [Grey.Darken4, BlueGrey.Medium, Cyan.Darken4]).Padding(20)
			.Row(row =>
			{
				row.RelativeItem().Column(col =>
				{
					col.Item().Text("INVOICE")
						.SemiBold()
						.FontSize(24)
						.FontColor(Colors.White)
						.FontFamily("Times New Roman");

					col.Item().Text($"{_order.OrderNumber}")
						.FontSize(14)
						.FontColor(LightGreen.Accent3)
						.FontFamily("Times New Roman")
						.ExtraBold();

					if (installment != null)
					{
						col.Item().PaddingTop(10).Row(innerRow =>
						{
							innerRow.AutoItem().PaddingLeft(8).Element(container =>
							{
								var size = 30;
								container
									.Width(size)
									.Height(size)
									.Background(Grey.Darken3)
									.Border(1)
									.BorderColor(Colors.White)
									.CornerRadius(size / 2)
									.AlignMiddle()
									.AlignCenter()
									.Padding(0)
									.Text(txt =>
									{
										txt.Span("N").FontSize(12)
									.FontColor(LightGreen.Accent2)
									.FontFamily("Times New Roman")
									.ExtraBold();
										txt.Span("o").Superscript().FontSize(12)
									.FontColor(LightGreen.Accent2)
									.FontFamily("Times New Roman")
									.ExtraBold();
										txt.Span(installment.InstallmentNumber.ToString()).FontSize(14)
									.FontColor(LightGreen.Accent2)
									.FontFamily("Times New Roman")
									.ExtraBold();
									});
							});
						});
					}
				});

				var logoPath = Path.Combine(
					AppContext.BaseDirectory,
					"img",
					"orchware_logo.png"
				);

				row.ConstantItem(100).AlignMiddle().Height(60)
					.Image(logoPath);
			});


			page.Content().PaddingVertical(10).PaddingHorizontal(50).Column(col =>
			{
				col.Spacing(10);

				col.Item().Row(row => {
					row.RelativeItem(300).BorderBottom(0.3f).BorderLinearGradient(20, [Cyan.Darken4, Colors.Blue.Darken1, Cyan.Darken4]).CornerRadiusBottomLeft(15).Column(col =>
					{
						col.Item().Text("ORCHWARE ltd").Bold().FontSize(18).FontColor(Grey.Darken4).FontFamily("Times New Roman");
					});
					row.RelativeItem(200).BorderBottom(0.4f).BorderLinearGradient(0, [Cyan.Darken4, Colors.Blue.Darken1]).CornerRadiusBottomRight(50).PaddingRight(19).Column(col =>
					{
						col.Item().Text("BILL TO").Bold().FontSize(18).FontColor(Grey.Darken4).FontFamily("Times New Roman");
					});
				});

				col.Item().Row(row => {

					row.RelativeItem(300).BorderBottom(1).BorderColor(Grey.Darken4).PaddingBottom(20).Column(col =>
					{
						col.Spacing(3);
						col.Item().Text(txt =>
						{
							txt.Span("Address: ").Bold().FontSize(11);
							txt.Span("######").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("hone Number: ").Bold().FontSize(11);
							txt.Span("######").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("E-mail: ").Bold().FontSize(11);
							txt.Span("######@###.##").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Tax Number: ").Bold().FontSize(11);
							txt.Span("######").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Registration Number: ").Bold().FontSize(11);
							txt.Span("#####-#####").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Bank Name: ").Bold().FontSize(11);
							txt.Span("#####").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Account Holder: ").Bold().FontSize(11);
							txt.Span("#####").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("IBAN: ").Bold().FontSize(11);
							txt.Span("#####").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("SWIFT/BIC: ").Bold().FontSize(11);
							txt.Span("#####").FontSize(10);
						});
					});

					row.RelativeItem(200).BorderBottom(1).BorderColor(Grey.Darken4).Column(col =>
					{
						col.Spacing(3);
						
						col.Item().Text(txt => {
							txt.Span("Order Date: ").Bold().FontSize(11);
							txt.Span($"{_order.OrderedDate:dd.MM.yyyy}").FontSize(10);
							
						});

						col.Item().Text(txt => {
							txt.Span("Buyer: ").Bold().FontSize(11);
							txt.Span($"{_order.Buyer.Name}").FontSize(10);
						});

						col.Item().Text($" -      work as {_order.Buyer.JobTitle}        -").Italic().FontSize(10);

						col.Item().Text(txt => {
							txt.Span("Buyer Personal Email: ").Bold().FontSize(11);
							txt.Span($"{_order.Buyer.PersonalEmail}").FontSize(10);
						});

						col.Item().Text("|-------------------------------|-------------------------------------|").FontSize(6).FontColor(Colors.Grey.Darken1);

						col.Item().Text(txt => {
							txt.Span("Company: ").Bold();
							txt.Span($"{_order.Buyer.CompanyName} - {_order.Buyer.CompanyLocation}");
						});

						col.Item().Text(txt => {
							txt.Span("Address: ").Bold().FontSize(11);
							txt.Span($"{_order.Buyer.CompanyAddress}, {_order.Buyer.CompanyCity}").FontSize(10);
						});

						col.Item().Text(txt => {
							txt.Span("Email: ").Bold().FontSize(11);
							txt.Span($"{_order.Buyer.CompanyEmail}").FontSize(10);
						});

						col.Item().Text(txt => {
							txt.Span("Phone: ").Bold().FontSize(11);
							txt.Span($"{_order.Buyer.CompanyPhoneNumber}").FontSize(10);
						});
					});
				});

				col.Item().Row(row =>
				{
					row.RelativeItem(400).BorderBottom(1).BorderColor(Grey.Darken4).PaddingBottom(20).Column(col =>
					{
						col.Spacing(3);

						col.Item().Text(txt =>
						{
							txt.Span("Payment Method: ").Bold().FontSize(11);
							txt.Span($"{(_order.PaymentInfo.WillBePaidInFull ? "Full payment" : "Installments")}").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Payment Date: ").Bold().FontSize(11);
							txt.Span($"{_order.PaymentInfo.PaymentDate:dd.MM.yyyy}").FontSize(10);
						});
						col.Item().Text(txt =>
						{
							txt.Span("Amount: ").Bold().FontSize(11);
							txt.Span($"{_order.Amount:C}").FontSize(10);
						});
						if (_order.Discount.HasValue)
						{
							col.Item().Text(txt =>
							{
								txt.Span("Discount: ").Bold().FontSize(11);
								txt.Span($"{_order.Discount}%").FontSize(10);
							});
						}

						col.Item().Background(BlueGrey.Lighten4).Padding(4).CornerRadius(5).Text($"Total Amount: {_order.TotalAmount:C}").ExtraBold();
					});

					if (!_order.PaymentInfo.WillBePaidInFull && installment is not null)
					{
						row.Spacing(4);
						row.RelativeItem(300)
						.Rotate(3).BorderBottom(1).BorderColor(Grey.Darken4).PaddingBottom(20).AlignCenter()
						.Background(Colors.White).Shadow(new BoxShadowStyle
						{
							Color = Colors.Grey.Medium,
							Blur = 5,
							Spread = 5,
							OffsetX = 5,
							OffsetY = 5
						}).CornerRadius(12).Padding(10).Column(col =>
						{
							col.Item().Text($"{installment.InstallmentCode}").ExtraBold().Underline().DecorationDashed();
							col.Item().PaddingTop(4).Text($"Total Installments: {_order.PaymentInfo.NumberOfInstallments}").FontSize(10);
							col.Item().Text($"Installment: {installment.InstallmentNumber}").FontSize(10);
							col.Item().Text($"Installment Amount: {installment.Amount:C}").FontSize(10);
							col.Item().Text($"Installment Due Date: {installment.DueDate:dd.MM.yyyy}").FontSize(10);
						});
					}
				});

				// Product Table
				col.Item().Text("Product Details:").ExtraBold().FontSize(16);
				col.Item().Table(table =>
				{
					table.ColumnsDefinition(c =>
					{
						c.RelativeColumn(3);
						c.RelativeColumn(2);
						c.RelativeColumn(2);
						c.RelativeColumn(2);
						c.RelativeColumn(2);
					});

					// Header
					table.Header(header =>
					{
						header.Cell().Background(Cyan.Darken4).Element(CellStyle).Text("Product").ExtraBold().FontColor(White);
						header.Cell().Background(Cyan.Darken4).Element(CellStyle).Text("Seasonal Fruit").ExtraBold().FontColor(White);
						header.Cell().Background(Cyan.Darken4).Element(CellStyle).AlignRight().Text("Quantity").ExtraBold().FontColor(White);
						header.Cell().Background(Cyan.Darken4).Element(CellStyle).AlignRight().Text("Price/Unit").ExtraBold().FontColor(White);
						header.Cell().Background(Cyan.Darken4).Element(CellStyle).AlignRight().Text("Total").ExtraBold().FontColor(White);
					});

					// Rows
					foreach (var detail in _order.OrderDetails)
					{
						table.Cell().Element(CellStyle).Text(detail.Product.Name);
						table.Cell().Element(CellStyle).Text(detail.Product.SeasonalFruits);
						table.Cell().Element(CellStyle).AlignRight().Text($"{detail.Quantity} {detail.UnitOfMeasure}");
						table.Cell().Element(CellStyle).AlignRight().Text($"{detail.PricePerUnit:C}");
						table.Cell().Element(CellStyle).AlignRight().Text($"{detail.TotalPrice:C}");
					}

					static IContainer CellStyle(IContainer container) =>
						container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
				});

				string installmentPay = _order.PaymentInfo.WillBePaidInFull ? $"{_order.TotalAmount:C} by {_order.PaymentInfo.PaymentDate:dd.MM.yyyy}" : $"{_order.PaymentInfo.AmountPerInstallment:C} by {installment!.DueDate:dd.MM.yyyy}";
				col.Item().Background(BlueGrey.Lighten4).Padding(4).Text($"You have to pay the current installment of: {installmentPay}").ExtraBold().FontSize(10);

				col.Item().PaddingTop(10)
							.LineHorizontal(1)
							.LineDashPattern([4f, 4f])
							.LineGradient([Grey.Lighten2, Grey.Darken4, Grey.Lighten2, BlueGrey.Medium]);

				// Bank Signature Line
				col.Item().PaddingTop(3).Column(section =>
				{
					section.Item().Text("Payment confirmation").SemiBold().FontSize(14);

					section.Item().PaddingTop(10).Text("Authorized signature: __________________________").FontSize(12).Italic();
					section.Item().PaddingTop(5).Text("Name and surname: ________________________________________________").FontSize(12).Italic();
					section.Item().PaddingTop(5).Text("Bank representative's signature: ______________________________________________").FontSize(12).Italic();
					section.Item().PaddingTop(5).Text("Official bank stamp: _______________________________________________").FontSize(12).Italic();
					section.Item().PaddingTop(5).Text("Date of payment: ______________________________________________").FontSize(12).Italic();
				});
			});

			page.Footer().AlignCenter().Text($"Generated on {DateTime.UtcNow:dd.MM.yyyy HH:mm}");
		});
	}
}
