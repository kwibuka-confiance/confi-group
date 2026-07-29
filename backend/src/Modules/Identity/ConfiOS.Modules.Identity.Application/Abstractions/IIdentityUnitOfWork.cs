using ConfiOS.BuildingBlocks.Application.Abstractions;

namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>
/// The Identity module's transaction boundary. Module-specific so it resolves to this
/// module's <c>DbContext</c> and never collides with another module's unit of work in the
/// shared container.
/// </summary>
public interface IIdentityUnitOfWork : IUnitOfWork;
