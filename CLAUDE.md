# CLAUDE.md

## Project

ConfiOS is a cloud-native Commerce Operating System built by Confi Group Ltd.

The first production customer is KwaConfi Depot. The software must solve real distribution and retail workflows before being generalized as SaaS.

## Product Direction

ConfiOS must remain industry-agnostic.

Never hardcode beverage-specific assumptions in the core platform. Use generic concepts such as:

- Product
- Category
- Brand
- Supplier
- Customer
- Warehouse
- Sales Order
- Purchase Order
- Payment
- Delivery

Industry-specific rules belong in optional modules.

## Architecture Principles

- Modular monolith first
- Clean Architecture
- Domain-Driven Design where it adds clarity
- Workflow-first development
- Multi-tenancy from day one
- Security by design
- Localization from day one
- Auditability for sensitive operations
- Event-driven integration inside the modular monolith
- Microservices only when scale or team boundaries justify them

## Technology Stack

### Frontend

- Flutter
- Material 3
- Responsive layouts
- Android, iOS, Web, Windows, and macOS where practical

### Backend

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis when needed
- OpenTelemetry
- Serilog
- Docker

## Non-Negotiable Rules

1. No business logic in controllers.
2. No business logic in Flutter widgets.
3. No hardcoded visible strings.
4. Every business entity must be tenant-scoped.
5. Sensitive actions require confirmation and audit logging.
6. Completed sales are immutable; use reversals or refunds.
7. Inventory may not become negative unless an explicit future policy allows controlled backorders.
8. Backend errors must expose stable machine-readable codes.
9. Use ISO currency codes and locale-aware formatting.
10. Every major architectural decision must be documented as an ADR.

## Supported Languages

MVP:

- English
- Kinyarwanda
- French

Future:

- Swahili
- Portuguese
- Arabic

## Implementation Order

1. Identity and tenant management
2. Catalog
3. Inventory
4. Customers and suppliers
5. Sales
6. Purchases
7. Expenses and reports
8. Notifications
9. Delivery
10. Payments
11. WhatsApp
12. AI
13. Public SaaS billing
14. Marketplace

## Commit Style

Use conventional commits:

- `feat:`
- `fix:`
- `docs:`
- `refactor:`
- `test:`
- `perf:`
- `chore:`

## Definition of Done

A feature is not complete until it has:

- Requirement reference
- Business rules
- Authorization checks
- Tenant isolation
- Validation
- Localization
- Audit behavior where required
- Automated tests
- Error states
- Documentation updates
