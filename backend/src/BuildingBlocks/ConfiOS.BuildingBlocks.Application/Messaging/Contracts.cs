using ConfiOS.BuildingBlocks.Domain.Errors;

namespace ConfiOS.BuildingBlocks.Application.Messaging;

/// <summary>Marker for a request that changes state.</summary>
public interface ICommand;

/// <summary>A state-changing request that returns a value.</summary>
/// <typeparam name="TResponse">Value produced on success.</typeparam>
public interface ICommand<TResponse>;

/// <summary>A read-only request.</summary>
/// <typeparam name="TResponse">Value produced on success.</typeparam>
public interface IQuery<TResponse>;

/// <summary>Handles a command that returns no value.</summary>
/// <typeparam name="TCommand">Command handled.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>Handles a command that returns a value.</summary>
/// <typeparam name="TCommand">Command handled.</typeparam>
/// <typeparam name="TResponse">Value produced on success.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>Handles a query.</summary>
/// <typeparam name="TQuery">Query handled.</typeparam>
/// <typeparam name="TResponse">Value produced on success.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
