# .NET Standards

## Structure

Suggested:

```text
src/
  BuildingBlocks/
  Modules/
    Identity/
    Catalog/
    Inventory/
    Sales/
  Api/
tests/
```

## Rules

- Use nullable reference types.
- Use analyzers.
- Use dependency injection.
- Use cancellation tokens.
- Avoid static mutable state.
- Keep EF Core configurations separate.
- Use migrations.
- Use transactions around consistency boundaries.
- Use strongly typed IDs where practical.
- Keep domain models persistence-ignorant.

## Validation

Use application-level validation plus domain invariants.

## Logging

Use structured logging.

Never log:

- Passwords
- OTPs
- Full payment credentials
- Secrets
- Sensitive personal data unnecessarily
