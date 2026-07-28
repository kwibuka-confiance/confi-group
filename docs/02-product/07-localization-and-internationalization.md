# Localization and Internationalization

## Purpose

Ensure ConfiOS supports multiple languages and regional formats from the beginning.

## Initial Languages

- English
- Kinyarwanda
- French

Future:

- Swahili
- Portuguese
- Arabic

## Requirements

- Users select a preferred language.
- Tenants define a default language.
- Device language may be detected at first launch.
- Users may override the tenant default.
- Notifications and generated documents use the recipient locale.
- No visible frontend text may be hardcoded.

## Frontend Rule

Incorrect:

```dart
Text('Complete Sale')
```

Correct:

```dart
Text(context.l10n.completeSale)
```

## Localization Scope

- Buttons
- Labels
- Validation errors
- Confirmation dialogs
- Dates and times
- Numbers
- Currency
- Receipts
- Invoices
- Reports
- Email
- SMS
- WhatsApp responses
- Product translations

## Product Translation Model

Use scalable translation records rather than language-specific columns.

Example:

- Product
- ProductTranslation
  - ProductId
  - LanguageCode
  - Name
  - Description

## Backend Errors

Backend responses must expose stable codes.

Example:

```json
{
  "code": "INSUFFICIENT_STOCK",
  "message": "Insufficient stock.",
  "details": {
    "availableQuantity": 4,
    "requestedQuantity": 10
  }
}
```

Clients translate based on `code`.

## Locale Headers

Clients may send:

```http
Accept-Language: rw-RW
```

Recommended initial locale codes:

- `en-RW`
- `rw-RW`
- `fr-RW`

## Currency

Store:

- Numeric amount
- ISO currency code

Do not store formatted currency strings.

## Requirements IDs

- I18N-001: All user-facing content uses localization resources.
- I18N-002: MVP supports English, Kinyarwanda, and French.
- I18N-003: Backend exposes stable error codes.
- I18N-004: Generated documents respect locale.
- I18N-005: Product translations use extensible translation records.
