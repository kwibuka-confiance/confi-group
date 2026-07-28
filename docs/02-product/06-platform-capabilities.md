# Platform Capabilities

## Purpose

Define shared capabilities used by every ConfiOS module.

## Capabilities

- Authentication
- Authorization
- Tenant context
- Localization
- Confirmation framework
- Approval framework
- Audit logging
- Notifications
- File storage
- Search
- Reporting
- Feature flags
- Licensing
- Monitoring
- Caching
- Background jobs
- Number generation
- Document generation
- Error handling

## Principle

Modules consume shared capabilities rather than implementing duplicate versions.

Example:

A future pharmacy module automatically receives:

- Authentication
- Permissions
- Localization
- Audit
- Notifications
- Reports
- Approval workflows

without rebuilding those features.
