using ConfiOS.BuildingBlocks.Domain.Errors;

namespace ConfiOS.BuildingBlocks.Application.Messaging;

/// <summary>
/// Routes a command or query to its handler. Deliberately hand-rolled and tiny: it keeps
/// the use-case entry point uniform for cross-cutting concerns without taking on a
/// mediator library or its licence terms.
/// </summary>
public interface IDispatcher
{
    Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<Result<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default);

    Task<Result<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default);
}
