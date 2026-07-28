using ConfiOS.BuildingBlocks.Application.Validation;
using ConfiOS.BuildingBlocks.Domain.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.BuildingBlocks.Application.Messaging;

/// <summary>
/// Default <see cref="IDispatcher"/>. Resolves the handler, runs any registered validator
/// first, and returns a failed <see cref="Result"/> rather than throwing when validation
/// fails.
/// </summary>
/// <remarks>
/// Handler lookup for the value-returning overloads goes through reflection once per
/// closed generic type, because the caller supplies the request as an interface. If this
/// ever shows up in a profile, cache the resolved invoker per request type.
/// </remarks>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public async Task<Result> SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var validation = Validate(command);
        if (validation is not null)
        {
            return Result.Failure(validation);
        }

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }

    public Task<Result<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return InvokeAsync<TResponse>(
            command,
            typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse)),
            cancellationToken);
    }

    public Task<Result<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return InvokeAsync<TResponse>(
            query,
            typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)),
            cancellationToken);
    }

    private Task<Result<TResponse>> InvokeAsync<TResponse>(
        object request,
        Type handlerType,
        CancellationToken cancellationToken)
    {
        var validation = Validate(request);
        if (validation is not null)
        {
            return Task.FromResult(Result.Failure<TResponse>(validation));
        }

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync))
            ?? throw new InvalidOperationException($"Handler {handlerType} exposes no HandleAsync method.");

        return (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
    }

    private Error? Validate(object request)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
        var validator = _serviceProvider.GetService(validatorType);

        if (validator is null)
        {
            return null;
        }

        var method = validatorType.GetMethod(nameof(IValidator<object>.Validate))!;
        var result = (ValidationResult)method.Invoke(validator, [request])!;

        return result.IsValid ? null : result.ToError();
    }
}
