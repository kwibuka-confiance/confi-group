# Database Design

## Database

PostgreSQL

## Identifier Strategy

Use UUIDs for public and business identifiers.

## Common Columns

Business entities should normally include:

- Id
- TenantId
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- IsDeleted

Not every immutable ledger or event table requires soft deletion.

## Tenancy

Every tenant-owned table includes `TenantId`.

Use:

- Global query filters
- Composite unique indexes including TenantId
- Tenant-aware repositories
- Database constraints where practical

## Suggested Schemas

- identity
- catalog
- inventory
- sales
- procurement
- customer
- finance
- logistics
- audit
- platform

## Important Tables

### Identity

- Tenants
- Branches
- Users
- Roles
- Permissions
- RolePermissions
- UserRoles
- Sessions
- Devices

### Catalog

- Products
- ProductTranslations
- Categories
- Brands
- Units
- Barcodes
- Prices

### Inventory

- Warehouses
- StockBalances
- StockMovements
- StockAdjustments
- StockTransfers
- StockCounts

### Sales

- SalesOrders
- SalesOrderItems
- Invoices
- Refunds

### Procurement

- Suppliers
- PurchaseOrders
- PurchaseOrderItems
- GoodsReceipts

### Finance

- Payments
- Expenses
- CashSessions
- Reconciliations

## Financial Storage

Never use floating-point types for money.

Use decimal/numeric types with explicit precision.

Store currency code separately.

## Indexing

Index:

- TenantId
- TenantId + business number
- TenantId + SKU
- TenantId + status
- TenantId + CreatedAt
- Foreign keys
- Frequently searched customer contact fields

## Audit

Audit logs are append-only and must not be editable by normal users.
