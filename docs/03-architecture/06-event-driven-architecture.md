# Event-Driven Architecture

## Purpose

Reduce coupling between modules.

## Domain Events

Examples:

- StockReceived
- StockAdjusted
- SaleCompleted
- PaymentReceived
- DeliveryCompleted

## Initial Implementation

Use in-process domain events within the modular monolith.

Persist integration events using an outbox pattern when side effects must be durable.

## Example

`SaleCompleted` may trigger:

- Inventory reduction
- Revenue recording
- Customer history update
- Receipt generation
- Dashboard refresh
- Analytics update

## Rules

- Events use past tense.
- Events are immutable.
- Handlers must be idempotent where retries are possible.
- External messaging is introduced only when needed.
