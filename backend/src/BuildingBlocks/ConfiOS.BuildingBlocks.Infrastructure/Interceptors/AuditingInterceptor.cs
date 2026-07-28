using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConfiOS.BuildingBlocks.Infrastructure.Interceptors;

/// <summary>
/// Fills CreatedAt/CreatedBy and UpdatedAt/UpdatedBy on save, and turns hard deletes of
/// business records into archival (BR-005).
/// </summary>
/// <remarks>
/// Doing this here rather than in handlers means the columns cannot be skipped or forged
/// by a caller, which is the point of having them.
/// </remarks>
/// <param name="tenantContext">Tenant and user resolved for the current request.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class AuditingInterceptor(ITenantContext tenantContext, IClock clock) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null)
        {
            Apply(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null)
        {
            Apply(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void Apply(DbContext context)
    {
        var now = clock.UtcNow;
        var userId = tenantContext.IsResolved ? tenantContext.UserId?.Value : null;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.MarkCreated(now, userId);
                    break;

                case EntityState.Modified:
                    entry.Entity.MarkUpdated(now, userId);
                    entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                    break;

                default:
                    break;
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.MarkDeleted(now);

            if (entry.Entity is IAuditable auditable)
            {
                auditable.MarkUpdated(now, userId);
            }
        }
    }
}
