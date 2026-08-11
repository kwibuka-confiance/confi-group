using ConfiOS.BuildingBlocks.Application.Auditing;

namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>
/// The Identity module's audit log, bound to this module's <c>DbContext</c> so it
/// never resolves to another module's. See <see cref="IIdentityUnitOfWork"/>.
/// </summary>
public interface IIdentityAuditLogger : IAuditLogger;
