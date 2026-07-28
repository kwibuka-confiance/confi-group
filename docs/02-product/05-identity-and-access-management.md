# Identity and Access Management

## Purpose

Define authentication, authorization, sessions, devices, approvals, and security audit requirements.

## Principles

- Zero trust
- Least privilege
- Tenant isolation
- Audit sensitive actions
- Secure by default

## Login Methods

Initial:

- Email
- Username
- Phone number

Future:

- Google
- Microsoft
- Apple
- Enterprise SSO
- Passkeys

## Password Policy

- Minimum 12 characters
- Strong hashing with Argon2id or bcrypt
- Never store or log plain text passwords
- Password reset tokens must be short-lived and single-use

## MFA

Preferred:

- Authenticator app
- Passkeys in future

Fallback:

- Email OTP
- SMS OTP

## Sessions

A session contains:

- Session ID
- User ID
- Tenant ID
- Device ID
- IP address
- User agent
- Login time
- Last activity
- Expiry
- Revocation status

## Authorization

Use permissions, not role names alone.

Examples:

- SALE_CREATE
- SALE_REFUND
- PRODUCT_CREATE
- PRODUCT_ARCHIVE
- USER_INVITE
- REPORT_VIEW
- PAYMENT_APPROVE
- INVENTORY_ADJUST
- INVENTORY_TRANSFER

## Custom Roles

Tenants may create custom roles from available permissions.

## Critical Operations

May require:

- Confirmation
- Reason
- Reauthentication
- Transaction PIN
- Manager approval
- Audit logging

## Token Rules

- Short-lived access tokens
- Refresh token rotation
- Revocation support
- Refresh token reuse detection

## Lockout

After repeated failed attempts:

- Temporarily lock account
- Record audit event
- Notify user or administrator as configured

## Requirements

- AUTH-001: Passwords must be strongly hashed.
- AUTH-002: Users can view and revoke sessions.
- AUTH-003: Repeated failed logins trigger lockout.
- AUTH-004: Passwords are never reversible.
- AUTH-005: Permission checks cannot be bypassed.
- AUTH-006: Sessions expire automatically.
- AUTH-007: Sensitive operations require audit logging.
- AUTH-008: Tenant isolation is mandatory.
- AUTH-009: Feature licensing is checked during authorization.
- AUTH-010: Authentication UI and messages are localized.
