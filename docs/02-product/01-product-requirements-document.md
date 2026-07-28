# Product Requirements Document

## Product

ConfiOS

## Purpose

ConfiOS is a cloud-native Commerce Operating System designed to help distributors and wholesalers manage their businesses from a single platform.

## Product Vision

Create the most intelligent and practical commerce platform for African businesses.

## Initial Target Customers

- Beverage wholesalers
- Beverage depots
- Liquor stores

## Future Target Customers

- Restaurants
- Hotels
- Bars
- Pharmacies
- Hardware stores
- Agriculture suppliers
- FMCG distributors
- Supermarkets

## Problems

Customers struggle with:

- Manual inventory records
- Stock losses
- Incorrect pricing
- Weak customer records
- Poor cash control
- Slow reporting
- Manual WhatsApp ordering
- No forecasting
- Limited business insight

## MVP Goals

Allow a business owner to:

- Manage products
- Manage warehouses
- Receive and adjust stock
- Sell retail and wholesale
- Track customers and suppliers
- Record expenses
- View daily performance
- Control users and permissions
- Use the platform in English, Kinyarwanda, or French

## Success Metrics

Within six months of deployment to KwaConfi Depot:

- Inventory accuracy above 98%
- Order processing time under two minutes
- Daily reporting under ten seconds
- Stock loss below 1%
- Failed order rate below 0.5%

## MVP Modules

- Identity
- Tenancy
- Catalog
- Inventory
- Sales
- Customers
- Suppliers
- Procurement
- Expenses
- Dashboard
- Localization
- Audit
- Confirmation and approval

## Non-Goals for MVP

- Payroll
- Manufacturing
- Full accounting
- Marketplace
- Advanced tax engine
- Multi-country support
- Public developer APIs
- AI automation
- WhatsApp ordering
- Payment processing

## Core Requirements

### PRD-001

All business data must be tenant-isolated.

### PRD-002

The MVP must support English, Kinyarwanda, and French.

### PRD-003

No visible frontend text may be hardcoded.

### PRD-004

Sensitive actions must require confirmation.

### PRD-005

Critical actions must be auditable.

### PRD-006

Completed sales must be immutable.

### PRD-007

Inventory must not become negative.

### PRD-008

Feature access must support licensing and feature flags.

### PRD-009

Every backend error must expose a stable machine-readable code.

### PRD-010

Every workflow must have documented acceptance criteria.
