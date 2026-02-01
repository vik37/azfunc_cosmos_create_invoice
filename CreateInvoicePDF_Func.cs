using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace azfunc_cosmos_create_invoice;

public class CreateInvoicePDF_Func
{
    private readonly ILogger<CreateInvoicePDF_Func> _logger;

    public CreateInvoicePDF_Func(ILogger<CreateInvoicePDF_Func> logger)
    {
        _logger = logger;
    }

    [Function("CreateInvoicePDF_Func")]
    public void Run([CosmosDBTrigger(
        databaseName: "orchwaredb",
        containerName: "order",
        Connection = "CosmosConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<MyDocument> input)
    {
        if (input != null && input.Count > 0)
        {
            _logger.LogInformation("Documents modified: " + input.Count);
            _logger.LogInformation("First document Id: " + input[0].id);
        }
    }
}

public class MyDocument
{
    public string id { get; set; }

    public string Text { get; set; }

    public int Number { get; set; }

    public bool Boolean { get; set; }
}