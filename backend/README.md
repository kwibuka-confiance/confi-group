# ConfiOS Backend

ASP.NET Core modular monolith. One deployable host, strict module boundaries, multi-tenant from
the first table.

See [ADR-006](../docs/adr/ADR-006-backend-project-structure.md) for why the solution is laid out
this way.

## Requirements

- .NET SDK 10.0
- Docker (for PostgreSQL and Redis)

## Running locally

```bash
# Start PostgreSQL, Redis and a trace collector
docker compose -f ../infrastructure/docker/compose.yaml up -d postgres redis jaeger

# Run the API
dotnet run --project src/Api/ConfiOS.Api
```

The API listens on <http://localhost:5080>. OpenAPI is served at `/openapi/v1.json` in
development, health at `/health`, and traces are at <http://localhost:16686>.

To run everything in containers instead:

```bash
docker compose -f ../infrastructure/docker/compose.yaml up --build
```

## Building and testing

```bash
dotnet build
dotnet test
```

Warnings are errors. That is deliberate: the analyzer set encodes several of the rules in
`CLAUDE.md`, and a warning that can be ignored will be.

## Layout

| Path | Contains |
|---|---|
| `src/BuildingBlocks/*` | Shared kernel: primitives, dispatcher, tenant context, EF base context, API envelopes |
| `src/Modules/<Module>/*` | One module, in four layers |
| `src/Api/ConfiOS.Api` | The host. Composes modules, owns authentication and observability |
| `tests/ConfiOS.ArchitectureTests` | Enforces layering, module boundaries and tenant scoping |

Modules currently in the solution:

- **Identity** — tenants, branches, users, roles, permissions. Implemented.
- **Catalog**, **Inventory**, **Sales** — registered and wired to their schemas, no aggregates yet.

## How the platform rules are enforced

| Rule | Where |
|---|---|
| Every business entity is tenant-scoped | `TenantDbContext` applies the filter by convention; `TenantIsolationTests` fails the build if an aggregate opts out |
| No business logic in controllers | Endpoints call one dispatcher method and map the result |
| No hardcoded visible strings | Errors carry codes only; messages come from `ErrorMessages.*.resx` in en, rw and fr |
| Stable machine-readable error codes | `ErrorCodes` and each module's own code class; the envelope always carries `code` |
| Sensitive actions are audited | `IAuditLogger` writes into the caller's transaction, to an append-only table |
| Money is never floating point | `Money` wraps `decimal` plus an ISO 4217 `Currency`, and rejects cross-currency arithmetic |
| Domain events survive their transaction | Raised on aggregates, drained into the outbox during `SaveChangesAsync` |

## Adding a module

1. Create four projects under `src/Modules/<Module>/` following the existing shape.
2. Add a `<Module>DbContext : TenantDbContext` with its own schema constant.
3. Implement `IModule`: register the context, repositories, handlers and validators, then map
   endpoints.
4. Reference the module's `Api` project from `ConfiOS.Api` and add its assembly to `AddModules`.
5. Add the module's error codes to the resource files in all three languages.

Do not reference another module's projects. If two modules need to share something, it belongs
in `BuildingBlocks` or travels as a domain event.

## Configuration

| Setting | Notes |
|---|---|
| `ConnectionStrings:Postgres` | Required |
| `Jwt:SigningKey` | Required. Startup fails without it. Development value is in `appsettings.Development.json`; production comes from the environment |
| `Jwt:Issuer`, `Jwt:Audience` | Token validation |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | Optional. Without it the exporter is inert |

## Known gaps

This is a scaffold. The following are deliberately not done yet:

- **No migrations.** Run `dotnet ef migrations add Initial --context IdentityDbContext` against
  the Identity infrastructure project once the schema settles.
- **No sign-in endpoint.** Password hashing and the user model are in place; token issuance is not.
- **The outbox is not drained.** `OutboxProcessor` exists but no hosted service calls it.
- **Error messages for all modules live in one resource file** in `BuildingBlocks.Api`. Splitting
  them per module is worth doing once there are more modules.
- **`IAuditLogger` is registered by Identity only.** Each module needs its own registration bound
  to its own context.
- **The build has not been run in CI yet.** The scaffold was written without a compiler available;
  expect to fix a small number of analyzer complaints on the first `dotnet build`.
