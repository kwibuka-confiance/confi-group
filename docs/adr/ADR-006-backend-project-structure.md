# ADR-006 Backend Project Structure

## Status

Accepted

## Context

ADR-005 chose a modular monolith and ADR-002 chose Domain-Driven Design. Neither says how the
.NET solution is laid out. Without that decision written down, the first few modules will each
invent their own arrangement, and the boundaries the earlier ADRs depend on will exist only in
review comments.

We also need the structure to make module extraction cheap later, without paying for
microservices now.

## Decision

One solution, `backend/ConfiOS.sln`, with four project layers per module.

```text
backend/
  src/
    BuildingBlocks/
      ConfiOS.BuildingBlocks.Domain          primitives, value objects, errors
      ConfiOS.BuildingBlocks.Application     dispatcher, tenant context, abstractions
      ConfiOS.BuildingBlocks.Infrastructure  EF base context, outbox, audit
      ConfiOS.BuildingBlocks.Api             envelopes, middleware, module contract
    Modules/
      <Module>/
        ConfiOS.Modules.<Module>.Domain
        ConfiOS.Modules.<Module>.Application
        ConfiOS.Modules.<Module>.Infrastructure
        ConfiOS.Modules.<Module>.Api
    Api/
      ConfiOS.Api                            single deployable host
  tests/
```

Rules that follow from it:

1. Dependencies point inward. Domain references nothing but other domain code. Application
   references Domain. Infrastructure references Application. Api references Infrastructure.
2. The host references each module's `Api` project only, and reaches it through the `IModule`
   interface. It never references a module's Domain, Application or Infrastructure directly.
3. Each module owns one PostgreSQL schema and one `DbContext`. No context maps another
   module's tables.
4. Modules communicate through published contracts and domain events, never by referencing
   each other's types.
5. Tenant filtering, audit stamping and outbox writing live in `TenantDbContext`, applied by
   convention to every entity that declares the relevant interface.
6. Package versions are centralised in `Directory.Packages.props`; no project pins its own.

Architecture tests in `tests/ConfiOS.ArchitectureTests` enforce rules 1, 4 and the tenant-scoping
requirement, so a violation fails the build rather than waiting for review.

## Consequences

### Positive

- The dependency rule is machine-checked, not aspirational.
- A module can be lifted into its own service by replacing its `IModule` registration and its
  outbox transport; nothing else in the host changes.
- One schema per module means the database enforces the boundary as well as the code does.
- Tenant isolation is applied centrally, so a module cannot forget it.

### Negative

- Four projects per module is a lot of files for a module that starts small. The alternative,
  one project per module with folders, makes the dependency rule unenforceable, which is the
  thing this ADR exists to protect.
- Central package management means a module cannot unilaterally take a different version of a
  shared dependency. That is intended.
- The build is slower than a single project would be.

### Neutral

- The hand-rolled command dispatcher in `BuildingBlocks.Application` replaces a mediator
  library. It is roughly a hundred lines and avoids a dependency whose licence terms have
  changed once already. If a pipeline of cross-cutting behaviours is needed later, it is the
  place to add them.

## Related

- [ADR-002 Domain-Driven Design](ADR-002-domain-driven-design.md)
- [ADR-003 Shared Multi-Tenant Platform](ADR-003-multi-tenancy.md)
- [ADR-005 Modular Monolith First](ADR-005-modular-monolith-first.md)
- [Clean Architecture](../03-architecture/02-clean-architecture.md)
- [Modular Monolith](../03-architecture/03-modular-monolith.md)
