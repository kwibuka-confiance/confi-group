using ConfiOS.BuildingBlocks.Application.Abstractions;

namespace ConfiOS.Modules.Catalog.Application.Abstractions;

/// <summary>
/// The Catalog module's transaction boundary. Module-specific so it resolves to this
/// module's <c>DbContext</c> and never collides with another module's unit of work in the
/// shared container.
/// </summary>
public interface ICatalogUnitOfWork : IUnitOfWork;
