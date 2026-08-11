using ConfiOS.BuildingBlocks.Application.Auditing;

namespace ConfiOS.Modules.Catalog.Application.Abstractions;

/// <summary>
/// The Catalog module's audit log. Module-specific for the same reason as
/// <see cref="ICatalogUnitOfWork"/>: every module registers an audit logger bound to
/// its own <c>DbContext</c>, so a shared <c>IAuditLogger</c> resolves to whichever
/// module registered last.
/// </summary>
/// <remarks>
/// That collision is silent and costly. The entry is added to another module's
/// context and then dropped when this module saves, so the action succeeds with no
/// record of it — the opposite of what the audit log promises.
/// </remarks>
public interface ICatalogAuditLogger : IAuditLogger;
