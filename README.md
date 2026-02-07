# Orchware – Invoice PDF Generation (Azure Function)

This repository contains an **Azure Function** triggered by **Cosmos DB Change Feed**. It automates the billing process by generating PDF invoices whenever a new order is recorded.

## 🚀 Workflow
1. **Trigger:** Listens to changes in the `order` container within `orchwaredb`.
2. **Processing:** Maps the Cosmos document to the Order entity and generates a PDF invoice.
3. **Storage:** Uploads the generated PDF to **Azure Blob Storage**.
4. **Update:** Patches the original Cosmos document with the `PdfUrl` and updates the order status to `ReadyForEmail`,  
   triggering a downstream Email Service that sends the invoice (one email to the client and one to the manager).

## 🛠 Tech Stack
- **C# / .NET 8** (Isolated Worker)
- **Azure Functions** (CosmosDBTrigger)
- **Azure Cosmos DB** (NoSQL)
- **Azure Blob Storage**
- **QuestPDF / iTextSharp** (Invoice Generation)

## 📄 PDF Generation

The PDF invoices are **fully generated and designed in code**.

There is **no pre-made or static template** involved.  
The layout, styling, and content structure are programmatically defined using domain data.

This approach allows:
- Full control over layout and branding
- Dynamic sections based on order/payment rules
- Deterministic and reproducible invoice output

## ⚙️ Configuration
Make sure to set up the following in `local.settings.json`:
- `CosmosConnectionString`: Your Cosmos DB connection string.
- `BlobStorageConnection`: Your Azure Storage connection string.


This project contains an **Azure Function** responsible for **automatically generating PDF invoices/receipts**, storing them securely in **Azure Blob Storage**, and persisting a **time-limited SAS URL** back to **Azure Cosmos DB**.

The function is designed to be **idempotent**, **event‑driven**, and **cloud‑native**, following clean architecture and domain‑driven design principles.

---

## 🧠 High‑level Flow Description

1. **Cosmos DB Change Feed** triggers the function when an `Order` document is inserted or updated.
2. The function checks whether an invoice PDF has already been generated.
3. If not:

   * A PDF invoice/receipt is generated using domain data.
   * The PDF is saved to **private Azure Blob Storage**.
   * A **read‑only SAS URL** is generated (time‑limited).
   * The Cosmos DB `Order` document is patched with invoice metadata.

This guarantees:

* No duplicate PDFs
* No infinite trigger loops
* Secure file access

---

## 🧩 Architecture Overview

**Trigger**

* Azure Cosmos DB Trigger (Change Feed)

**Storage**

* Azure Blob Storage (private container)

**Data**

* Azure Cosmos DB (patch‑based updates)

**Security**

* Blob‑level SAS (read‑only, expiring)

---

## 🧾 Invoice Logic

* **Receipts** are generated for orders paid in full
* **Invoices** are generated for installment‑based payments
* File naming is deterministic to support idempotency

Example:

```
invoices/invoice_12345_01.pdf
receipts/invoice_12345.pdf
```

---

## 🔁 Idempotency Strategy

The function will **skip processing** if the invoice has already been generated:

```csharp
if (order.Invoice is not null && order.Invoice.PdfGenerated)
    continue;
```

This ensures:

* Safe replays from the Cosmos DB change feed
* No duplicate blobs
* No recursive triggers

---

## 🔐 Blob Security (SAS)

* Blobs are **private** by default
* A **read‑only SAS URL** is generated per invoice
* The SAS URL has an expiration (e.g. 7 days)

This allows:

* Secure email delivery
* Temporary client access
* No public storage exposure

---

## 📦 Environment Variables

The following environment variables are required:

| Name                     | Description                          |
| ------------------------ | ------------------------------------ |
| `CosmosConnectionString` | Connection string for Cosmos DB      |
| `BlobStorageConnection`  | Azure Blob Storage connection string |

---

## 🛠 Technologies Used

* .NET 8
* Azure Functions (Isolated Worker)
* Azure Cosmos DB
* Azure Blob Storage
* QuestPDF (PDF generation)

---

## 📌 Design Notes

* Uses **PATCH** operations instead of document replacement (RU‑efficient)
* No shared state between executions
* Separation of concerns between orchestration and document generation
* Designed for scale and replay safety

---

## 🚀 Deployment

1. Configure environment variables in Azure Function App
2. Deploy the function
3. Insert or update an `Order` document
4. Observe invoice generation via logs

---

## 📖 Status

This project is part of a **personal, public system design exploration** focused on:

* Event‑driven architectures
* Domain‑centric modeling
* Cloud‑native backend patterns

---

## 🧑‍💻 Author Viktor Zafirovski

Built and maintained as part of the **Orchware** project.

Public repository – free to explore and learn from.
