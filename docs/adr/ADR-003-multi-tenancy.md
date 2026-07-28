# ADR-003: Shared Multi-Tenant Platform

## Status

Accepted

## Decision

Use shared infrastructure and a shared PostgreSQL database with strict logical isolation through TenantId.

Support future dedicated databases for enterprise tenants.

## Consequences

- Lower initial cost
- Easier operations
- Strong tenant-filtering discipline required
- Clear enterprise migration path
