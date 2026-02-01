# Orchware Invoice PDF Generator

This repository contains an **Azure Function** triggered by **Cosmos DB Change Feed**. It automates the billing process by generating PDF invoices whenever a new order is recorded.

## 🚀 Workflow
1. **Trigger:** Listens to changes in the `order` container within `orchwaredb`.
2. **Processing:** Maps the Cosmos document to the Order entity and generates a PDF invoice.
3. **Storage:** Uploads the generated PDF to **Azure Blob Storage**.
4. **Update:** Patches the original Cosmos document with the `PdfUrl` and updates status to `ReadyForEmail`.

## 🛠 Tech Stack
- **C# / .NET 8** (Isolated Worker)
- **Azure Functions** (CosmosDBTrigger)
- **Azure Cosmos DB** (NoSQL)
- **Azure Blob Storage**
- **QuestPDF / iTextSharp** (Invoice Generation)

## ⚙️ Configuration
Make sure to set up the following in `local.settings.json`:
- `CosmosConnectionString`: Your Cosmos DB connection string.
- `BlobStorageConnectionString`: Your Azure Storage connection string.
