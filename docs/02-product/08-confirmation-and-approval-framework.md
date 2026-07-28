# Confirmation and Approval Framework

## Purpose

Protect users from destructive, financial, inventory-changing, or difficult-to-reverse actions.

## Confirmation Levels

### Informational

Used for low-risk actions such as leaving with unsaved changes.

### Warning

Used for stock adjustments, product archival, or operational changes.

### Critical

Used for payment reversal, large refunds, permission changes, or sensitive administration.

## Confirmation Content

Every dialog should contain:

- Clear title
- Exact action
- Consequence
- Cancel action
- Specific confirmation action

Avoid vague text such as only “Are you sure?”

Preferred:

> Are you sure you want to refund this sale? Inventory, payment records, and the customer balance will be updated.

## Critical Requirements

Critical operations may require:

- Reason
- Password confirmation
- Transaction PIN
- Manager approval
- Owner approval
- Audit log
- Idempotency protection

## Actions Requiring Confirmation

- Complete sale
- Refund sale
- Reverse payment
- Archive product
- Adjust inventory
- Record damaged stock
- Transfer stock
- Approve discount
- Change roles or permissions
- Revoke sessions
- Cancel purchase order

## Requirements IDs

- UX-001: Sensitive actions require confirmation.
- UX-002: Confirmation text states exact consequences.
- UX-003: Critical actions may require approval or reauthentication.
- UX-004: Routine actions should not trigger unnecessary confirmations.
- UX-005: Every critical operation is audit logged.
