# Flutter Standards

## Structure

Use feature-first organization.

```text
lib/
  core/
  shared/
  features/
    inventory/
      data/
      domain/
      presentation/
```

## State Management

Use Bloc or Cubit consistently.

## Rules

- No business logic in widgets.
- No hardcoded strings.
- Use localization resources.
- Use typed routes.
- Handle loading, empty, error, and success states.
- Support responsive layouts.
- Support dark mode.
- Keep widgets small and composable.
- Use dependency injection consistently.

## Networking

- Centralize API client configuration.
- Use interceptors for auth, tenant context, locale, and trace IDs.
- Map backend error codes into localized client messages.
