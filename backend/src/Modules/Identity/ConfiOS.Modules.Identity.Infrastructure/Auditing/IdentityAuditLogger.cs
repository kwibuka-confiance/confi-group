using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Infrastructure.Auditing;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;

namespace ConfiOS.Modules.Identity.Infrastructure.Auditing;

/// <summary>
/// Binds the shared audit logger to the Identity context, so an entry commits in the
/// same transaction as the change it describes.
/// </summary>
/// <param name="context">Identity database context.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class IdentityAuditLogger(IdentityDbContext context, IClock clock)
    : IIdentityAuditLogger
{
    private readonly AuditLogger<IdentityDbContext> _inner = new(context, clock);

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default) =>
        _inner.RecordAsync(entry, cancellationToken);
}
