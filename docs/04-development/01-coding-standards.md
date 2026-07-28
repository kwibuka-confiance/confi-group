# Coding Standards

## General

- Prefer readable code.
- Keep methods focused.
- Use descriptive names.
- Avoid God classes.
- Prefer composition over inheritance.
- Avoid duplicate business rules.
- Keep domain logic out of UI and controllers.
- Use asynchronous APIs correctly.
- Handle cancellation tokens in backend operations.

## Naming

Use business language.

Good:

- ReceiveStock
- CompleteSale
- ArchiveProduct
- TransferInventory

Avoid:

- UpdateData
- ProcessThing
- InsertInventory

## Error Handling

- Use domain-specific exceptions or result types.
- Do not expose stack traces to clients.
- Always include trace IDs.
- Log failures with structured context.

## Documentation

Public APIs and important domain rules must be documented.
