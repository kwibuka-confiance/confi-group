# API Standards

## Style

Use RESTful HTTP APIs initially.

## Versioning

Use URL or header-based versioning consistently.

Example:

```http
/api/v1/products
```

## Response Format

Successful response:

```json
{
  "data": {},
  "meta": {},
  "traceId": "..."
}
```

Error response:

```json
{
  "code": "INSUFFICIENT_STOCK",
  "message": "Insufficient stock.",
  "details": {},
  "traceId": "..."
}
```

## Rules

- Use nouns for resources.
- Use explicit action endpoints only for business commands.
- Use pagination for lists.
- Use idempotency keys for sensitive operations.
- Include correlation IDs.
- Validate tenant context.
- Validate permissions.
- Validate feature license.

## Business Commands

Examples:

- `POST /api/v1/inventory/receipts`
- `POST /api/v1/stock-transfers/{id}/confirm`
- `POST /api/v1/sales/{id}/complete`
- `POST /api/v1/payments/{id}/reverse`

## Localization

Clients send `Accept-Language`.

Backend returns stable error codes regardless of locale.

## Security

Never trust tenant IDs sent directly by clients without validating membership.

## OpenAPI

Every endpoint must be documented through OpenAPI.
