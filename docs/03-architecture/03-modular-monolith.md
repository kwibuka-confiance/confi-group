# Modular Monolith

## Purpose

Define how ConfiOS remains one deployable application while preserving strong module boundaries.

## Rules

- Each module owns its domain model.
- Each module owns its application services.
- Cross-module data access is forbidden.
- Modules interact through contracts and events.
- Shared kernel must remain minimal.
- Database schemas may be separated by module.

## Suggested Modules

- ConfiOS.Identity
- ConfiOS.Catalog
- ConfiOS.Inventory
- ConfiOS.Sales
- ConfiOS.Procurement
- ConfiOS.Customers
- ConfiOS.Finance
- ConfiOS.Logistics
- ConfiOS.Notifications
- ConfiOS.Localization

## Extraction Readiness

A module may become a microservice only when:

- Independent scaling is required
- Team ownership requires deployment independence
- Reliability boundaries justify separation
- External integration complexity demands isolation
- Operational benefits exceed added complexity
