# Domain-Driven Design

## Purpose

Define the business model and language of ConfiOS.

## Ubiquitous Language

Use business language consistently.

| Avoid | Use |
|---|---|
| Insert inventory | Receive stock |
| Delete product | Archive product |
| Update quantity | Adjust stock |
| Insert payment | Record payment |
| Move quantity | Transfer stock |

## Bounded Contexts

- Identity
- Catalog
- Inventory
- Sales
- Procurement
- Customer
- Finance
- Logistics
- Notifications
- Localization
- AI

## Core Entities

- Tenant
- Branch
- User
- Product
- Customer
- Supplier
- Warehouse
- Sales Order
- Purchase Order
- Payment
- Delivery
- Expense

## Value Objects

### Money

- Amount
- Currency

### Address

- Country
- Province
- District
- Sector
- Cell
- Village
- Street

### Contact

- Phone number
- Email address

## Aggregates

### Product Aggregate

- Product
- Product translations
- Barcodes
- Prices
- Unit

### Customer Aggregate

- Customer
- Contacts
- Addresses
- Credit limit
- Notes

### Sales Order Aggregate

- Sales order
- Order items
- Discounts
- Payment references
- Invoice reference
- Delivery reference

## Domain Services

- Inventory Allocation Service
- Pricing Service
- Payment Coordination Service
- Tax Calculation Service
- Number Generation Service

## Domain Events

- TenantCreated
- ProductCreated
- ProductArchived
- SupplierRegistered
- StockReceived
- StockAdjusted
- StockTransferred
- CustomerCreated
- SaleCompleted
- InvoiceGenerated
- PaymentReceived
- ExpenseRecorded
- DeliveryAssigned
- DeliveryCompleted
- LanguageChanged

## Invariants

- Inventory quantity must not be negative.
- SKU must be unique per tenant.
- Barcode must be unique within its defined scope.
- A warehouse belongs to one branch.
- A branch belongs to one tenant.
- Completed sales cannot return to draft.
- Completed deliveries cannot return to draft.
