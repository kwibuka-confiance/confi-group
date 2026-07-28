# Business Workflows

## Purpose

Define how ConfiOS supports real business operations.

Every screen, API, and database structure must support a documented workflow.

## Workflow Categories

### Business Setup

- Register business
- Configure business profile
- Configure branches
- Configure warehouses
- Configure languages
- Configure currencies
- Invite users
- Assign roles

### Product Management

- Create product
- Update product
- Archive product
- Create category
- Create brand
- Configure units
- Import products
- Print barcodes

### Purchasing

- Register supplier
- Create purchase order
- Receive stock
- Record shortages
- Record damaged goods
- Record supplier invoice
- Complete purchase

### Inventory

- View stock
- Receive stock
- Transfer stock
- Adjust stock
- Record damaged products
- Perform stock count
- Approve adjustment

### Retail Sales

- Create sale
- Add products
- Apply discount
- Receive payment
- Confirm sale
- Print receipt
- Refund sale

### Wholesale Sales

- Create quotation
- Convert quotation to order
- Reserve stock
- Generate invoice
- Receive payment
- Dispatch order
- Complete order

### Finance

- Record expense
- Approve expense
- Reconcile payment
- Close business day
- Generate reports

## Workflow Template

Each workflow must define:

- Goal
- Actors
- Preconditions
- Trigger
- Main flow
- Alternative flows
- Validation rules
- Confirmation message
- Audit events
- Notifications
- APIs
- UI screens
- Acceptance criteria

## Example: Receive Stock

### Goal

Increase inventory after supplier delivery.

### Actors

- Warehouse staff
- Branch manager

### Preconditions

- Supplier exists
- Purchase order exists
- Warehouse exists

### Main Flow

1. Open Receive Stock.
2. Select purchase order.
3. Scan or select products.
4. Verify quantities.
5. Record shortages or damaged items.
6. Confirm receiving.
7. Increase inventory.
8. Publish `StockReceived`.
9. Update purchase order status.
10. Refresh dashboard.

### Validation Rules

- Product must exist.
- Warehouse must exist.
- Quantity must be positive.
- Over-receipt requires approval.
- Expired goods cannot be accepted.

### Confirmation

> Receive the selected items into this warehouse? This operation will increase inventory.

### Audit Event

`StockReceived`

## Global Business Rules

- BR-001: No stock may exist without a warehouse.
- BR-002: Every stock movement must create an audit record.
- BR-003: Inventory may not become negative.
- BR-004: Completed sales cannot be edited.
- BR-005: Business records should be archived rather than deleted.
- BR-006: Every financial operation must be traceable.
- BR-007: Sensitive actions require confirmation.
- BR-008: Critical actions require audit logging.
- BR-009: Every business action belongs to a tenant.
- BR-010: Every user-visible message must support localization.
