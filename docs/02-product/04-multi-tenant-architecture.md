# Multi-Tenant Architecture

## Purpose

Define how multiple businesses securely use ConfiOS.

## Tenant Definition

A tenant is one independent business.

Examples:

- KwaConfi Depot
- Musanze Beverage Depot
- ABC Pharmacy
- Kigali Hardware

## Hierarchy

- Platform
- Tenant
- Branch
- Warehouse
- User assignment

## Isolation Rules

- Every business entity contains `TenantId`.
- Every authenticated request resolves tenant context.
- Every query is tenant-filtered.
- Every authorization decision checks tenant membership.
- AI data must never cross tenant boundaries.
- Audit records include tenant and branch.

## Branch Access

Users may be assigned to one or more branches.

Branch permissions never override tenant boundaries.

## Feature Licensing

Features are enabled by subscription and feature flags.

Example modules:

- Inventory
- Sales
- Purchasing
- Finance
- Delivery
- WhatsApp
- AI
- Marketplace

## Country Configuration

Each tenant defines:

- Country
- Currency
- Time zone
- Default language
- Fiscal year
- Number format
- Date format
- Tax configuration

## Initial Storage Strategy

Use a shared PostgreSQL database with strict logical isolation by `TenantId`.

Future enterprise customers may use:

- Dedicated database
- Dedicated storage
- Dedicated infrastructure

## Rules

- MT-001: Every business entity belongs to exactly one tenant.
- MT-002: Every authenticated request must contain tenant context.
- MT-003: Multi-tenant membership requires explicit authorization.
- MT-004: Users may belong to multiple branches.
- MT-005: Branch access cannot cross tenant boundaries.
- MT-006: Feature availability is controlled by licensing and feature flags.
- MT-007: Locale is configurable per tenant and overridable per user.
- MT-008: Generated documents must respect tenant branding and locale.
