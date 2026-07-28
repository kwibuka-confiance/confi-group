# Testing Strategy

## Test Types

- Domain unit tests
- Application use case tests
- Infrastructure integration tests
- API integration tests
- Flutter widget tests
- End-to-end tests
- Security tests
- Localization tests

## Priority Workflows

Must have strong coverage:

- Login
- Tenant isolation
- Stock receiving
- Stock transfer
- Sale completion
- Refund
- Payment recording
- Permission enforcement
- Critical confirmation flows

## Test Data

Use tenant-separated test fixtures.

Every cross-tenant test must prove isolation.
