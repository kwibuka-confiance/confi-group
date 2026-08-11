using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Infrastructure.Auditing;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Infrastructure.Persistence;

namespace ConfiOS.Modules.Catalog.Infrastructure.Auditing;

/// <summary>
/// Binds the shared audit logger to the Catalog context, so an entry commits in the
/// same transaction as the change it describes.
/// </summary>
/// <param name="context">Catalog database context.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class CatalogAuditLogger(CatalogDbContext context, IClock clock)
    : ICatalogAuditLogger
{
    private readonly AuditLogger<CatalogDbContext> _inner = new(context, clock);

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default) =>
        _inner.RecordAsync(entry, cancellationToken);
}
