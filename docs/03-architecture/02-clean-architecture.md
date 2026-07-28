# Clean Architecture

## Layers

### Domain

Contains:

- Entities
- Value objects
- Domain services
- Domain events
- Business rules

Must not depend on infrastructure.

### Application

Contains:

- Use cases
- Commands
- Queries
- Validation
- Interfaces
- Authorization coordination

### Infrastructure

Contains:

- Entity Framework Core
- PostgreSQL
- Redis
- Object storage
- External providers
- Email
- SMS
- WhatsApp
- Payment adapters

### API

Contains:

- HTTP endpoints
- Authentication middleware
- Tenant middleware
- Request models
- Response mapping

## Dependency Rule

Dependencies point inward.

Infrastructure may depend on Application and Domain.

Domain must not depend on Infrastructure.

## Controller Rule

Controllers should:

- Validate transport concerns
- Call one application use case
- Return mapped results

Controllers must not contain business logic.
