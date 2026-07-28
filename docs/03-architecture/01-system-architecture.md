# System Architecture

## Architectural Style

Use a modular monolith for the initial platform.

Reasons:

- Faster development
- Easier debugging
- Lower infrastructure complexity
- Strong transactional consistency
- Clear module boundaries
- Easier future extraction into services

## Major Components

### Client Applications

- Admin and owner app
- POS app
- Warehouse app
- Future customer app
- Future driver app

### Backend

- ASP.NET Core API
- Background worker
- Modular application services
- Domain modules
- Infrastructure adapters

### Data

- PostgreSQL
- Redis when needed
- Object storage for documents and files

### Observability

- OpenTelemetry
- Structured logging
- Metrics
- Distributed tracing
- Alerting

## Request Flow

Client

→ API

→ Authentication

→ Tenant resolution

→ Permission check

→ Feature license check

→ Application use case

→ Domain model

→ Persistence

→ Domain events

→ Response

## Module Boundaries

- Identity
- Catalog
- Inventory
- Sales
- Procurement
- Customers
- Finance
- Logistics
- Notifications
- Localization
- AI

## Integration Principles

- Prefer in-process module contracts initially.
- Use domain events for decoupled reactions.
- Use an outbox pattern before introducing external messaging.
- Introduce RabbitMQ only when asynchronous scaling justifies it.
