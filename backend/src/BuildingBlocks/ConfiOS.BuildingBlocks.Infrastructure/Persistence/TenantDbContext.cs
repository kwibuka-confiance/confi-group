using System.Linq.Expressions;
using System.Reflection;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ConfiOS.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base context for every module. Applies tenant and soft-delete query filters to all
/// entities that declare them, and drains domain events into the outbox on save.
/// </summary>
/// <remarks>
/// Filters are applied by convention here rather than by each module, because a module
/// that forgets one leaks another tenant's data. A module that genuinely needs to read
/// across tenants must say so explicitly with <c>IgnoreQueryFilters</c> in a reviewed query.
/// </remarks>
/// <param name="options">EF Core options supplied by the host.</param>
/// <param name="tenantContext">Tenant resolved for the current request.</param>
/// <param name="clock">Source of the current time.</param>
public abstract class TenantDbContext(
    DbContextOptions options,
    ITenantContext tenantContext,
    IClock clock) : DbContext(options), IUnitOfWork
{
    private static readonly MethodInfo BuildFilterMethod =
        typeof(TenantDbContext).GetMethod(nameof(BuildFilter), BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly IClock _clock = clock;

    /// <summary>
    /// Tenant for the current request, or <see cref="Guid.Empty"/> when none has been
    /// resolved. Empty is deliberate: an unresolved request then matches no rows rather
    /// than every row, so the filter fails closed.
    /// </summary>
    public Guid CurrentTenantId => TenantContext.IsResolved ? TenantContext.TenantId.Value : Guid.Empty;

    /// <summary>Schema this module's tables live in, for example <c>identity</c>.</summary>
    protected abstract string Schema { get; }

    protected ITenantContext TenantContext { get; } = tenantContext;

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        return transaction;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTenant();
        await WriteOutboxMessagesAsync(cancellationToken).ConfigureAwait(false);

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration(Schema));

        // Materialised first: configuring an entity type inside the loop can add others,
        // and the model's collection must not change while it is being enumerated.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            // Owned types are queried through their owner and inherit its filter. Setting
            // one on them directly is rejected by EF Core.
            if (entityType.IsOwned())
            {
                continue;
            }

            var clrType = entityType.ClrType;

            if (typeof(AggregateRoot).IsAssignableFrom(clrType))
            {
                // Domain events live in memory only; the outbox is what gets persisted.
                modelBuilder.Entity(clrType).Ignore(nameof(AggregateRoot.DomainEvents));
            }

            var isTenantScoped = typeof(ITenantScoped).IsAssignableFrom(clrType);
            var isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(clrType);

            if (!isTenantScoped && !isSoftDeletable)
            {
                continue;
            }

            var filter = (LambdaExpression)BuildFilterMethod
                .MakeGenericMethod(clrType)
                .Invoke(null, [this, isTenantScoped, isSoftDeletable])!;

            entityType.SetQueryFilter(filter);

            if (isTenantScoped)
            {
                modelBuilder.Entity(clrType).HasIndex(nameof(ITenantScoped.TenantId));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Builds the query filter for one entity type.
    /// </summary>
    /// <remarks>
    /// The lambda captures <paramref name="context"/> rather than a snapshot of the tenant
    /// id, so EF Core re-reads <see cref="CurrentTenantId"/> on every query instead of
    /// baking in whichever tenant happened to be current when the model was built.
    /// </remarks>
    private static LambdaExpression BuildFilter<TEntity>(
        TenantDbContext context,
        bool isTenantScoped,
        bool isSoftDeletable)
        where TEntity : class
    {
        if (isTenantScoped && isSoftDeletable)
        {
            return (Expression<Func<TEntity, bool>>)(entity =>
                EF.Property<Guid>(entity, nameof(ITenantScoped.TenantId)) == context.CurrentTenantId
                && !EF.Property<bool>(entity, nameof(ISoftDeletable.IsDeleted)));
        }

        if (isTenantScoped)
        {
            return (Expression<Func<TEntity, bool>>)(entity =>
                EF.Property<Guid>(entity, nameof(ITenantScoped.TenantId)) == context.CurrentTenantId);
        }

        return (Expression<Func<TEntity, bool>>)(entity =>
            !EF.Property<bool>(entity, nameof(ISoftDeletable.IsDeleted)));
    }

    /// <summary>
    /// Sets TenantId on new rows from the resolved context so a caller cannot write into
    /// another tenant by supplying a different value (MT-001).
    /// </summary>
    private void StampTenant()
    {
        if (!TenantContext.IsResolved)
        {
            // Provisioning a new tenant runs without a resolved context. Those aggregates
            // carry the tenant they were constructed with, so nothing needs stamping.
            return;
        }

        var tenantId = TenantContext.TenantId.Value;

        foreach (var entry in ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property(nameof(ITenantScoped.TenantId)).CurrentValue = tenantId;
            }
        }
    }

    /// <summary>
    /// Moves raised domain events into the outbox table in this same transaction, so an
    /// event can never be published for a change that was rolled back.
    /// </summary>
    private async Task WriteOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        var roots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        if (roots.Count == 0)
        {
            return;
        }

        var tenantId = TenantContext.IsResolved ? TenantContext.TenantId.Value : (Guid?)null;
        var messages = new List<OutboxMessage>();

        foreach (var root in roots)
        {
            messages.AddRange(root.DomainEvents.Select(domainEvent =>
                OutboxMessage.From(domainEvent, tenantId, _clock.UtcNow)));

            root.ClearDomainEvents();
        }

        await Set<OutboxMessage>().AddRangeAsync(messages, cancellationToken).ConfigureAwait(false);
    }
}
