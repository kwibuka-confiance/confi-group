# Confi Group Ltd

Confi Group Ltd is building technology and operating businesses that simplify commerce across Africa.

Our first operating business is **KwaConfi Depot**, a beverage wholesale and retail outlet with a small bar and billiards. Our first technology product is **ConfiOS**, a multi-tenant commerce operating system initially designed for beverage distributors and later adaptable to other industries.

## Repository Purpose

This repository is the source of truth for:

- Company vision and business model
- Product requirements
- Business workflows
- Domain model
- System architecture
- Security and multi-tenancy
- Localization and confirmation standards
- Development standards
- Product roadmap
- Future implementation

## Product Ecosystem

- **KwaConfi Depot** — beverage wholesale, retail, small bar, and billiards
- **Confi Distribution** — wholesale and delivery operations
- **Confi Technologies** — software development and platform operations
- **ConfiOS** — commerce and distribution operating system
- **Confi AI** — business intelligence and automation
- **Confi Pay** — future payments and reconciliation services
- **Confi Logistics** — future delivery and fleet services
- **Confi Marketplace** — future B2B commerce platform

## Initial MVP

The first ConfiOS release will support KwaConfi Depot with:

1. Tenant and user management
2. Products, categories, brands, units, and barcodes
3. Inventory receiving, transfers, adjustments, and stock movements
4. Wholesale and retail sales
5. Customers and suppliers
6. Expenses and basic profitability reports
7. Daily dashboard and audit logs
8. English, Kinyarwanda, and French localization
9. Confirmation and approval controls for sensitive operations

Payments, customer ordering, WhatsApp automation, delivery tracking, AI, and public SaaS billing will follow after the operational core is stable.

## Documentation

Start with:

- [Documentation Index](docs/index.md)
- [Project Instructions](CLAUDE.md)
- [Vision and Business Foundation](docs/01-company/01-vision-and-business-foundation.md)
- [Business Model](docs/01-company/02-business-model.md)
- [Product Requirements](docs/02-product/01-product-requirements-document.md)
- [Business Workflows](docs/02-product/02-business-workflows.md)
- [Domain-Driven Design](docs/02-product/03-domain-driven-design.md)
- [Multi-Tenant Architecture](docs/02-product/04-multi-tenant-architecture.md)
- [Identity and Access Management](docs/02-product/05-identity-and-access-management.md)
- [Backend README](backend/README.md)

## Technology Direction

- Flutter for client applications
- ASP.NET Core for backend services
- PostgreSQL for transactional data
- Redis for caching and distributed coordination when needed
- RabbitMQ only when justified by asynchronous workload growth
- Docker for local development and deployment
- Azure as the initial cloud target
- OpenTelemetry for observability

## Guiding Principle

> Build the simplest reliable system that solves real operational problems at KwaConfi Depot, then generalize proven capabilities for other businesses.
