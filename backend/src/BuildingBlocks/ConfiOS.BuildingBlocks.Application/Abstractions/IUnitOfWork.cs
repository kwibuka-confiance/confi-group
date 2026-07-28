namespace ConfiOS.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Commits a consistency boundary. One unit of work per use case: a handler that needs
/// two commits is describing two use cases.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens an explicit transaction for the rare case where several aggregates must
    /// change atomically.
    /// </summary>
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
