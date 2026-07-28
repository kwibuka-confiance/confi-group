using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Domain.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConfiOS.BuildingBlocks.Api.Middleware;

/// <summary>
/// Converts unhandled exceptions into the standard error envelope.
/// </summary>
/// <remarks>
/// A <see cref="DomainException"/> carries a stable code and is safe to surface. Anything
/// else is logged in full and returned as UNKNOWN_ERROR, so stack traces and connection
/// strings never reach a client.
/// </remarks>
/// <param name="next">Next middleware in the pipeline.</param>
/// <param name="logger">Logger for the failure.</param>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IErrorMessageLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (DomainException exception)
        {
            logger.LogWarning(
                exception,
                "Domain rule rejected the request. Code {ErrorCode}, trace {TraceId}.",
                exception.Error.Code,
                context.TraceIdentifier);

            await WriteAsync(context, exception.Error, localizer).ConfigureAwait(false);
        }
#pragma warning disable CA1031 // The top-level handler must catch everything.
        catch (Exception exception)
#pragma warning restore CA1031
        {
            logger.LogError(
                exception,
                "Unhandled exception. Trace {TraceId}.",
                context.TraceIdentifier);

            await WriteAsync(context, new Error(ErrorCodes.Unknown), localizer).ConfigureAwait(false);
        }
    }

    private static async Task WriteAsync(HttpContext context, Error error, IErrorMessageLocalizer localizer)
    {
        if (context.Response.HasStarted)
        {
            // Too late to change the response; the logged entry is the record of it.
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = ResultExtensions.ToStatusCode(error.Type);
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsJsonAsync(
            new Contracts.ApiError(
                error.Code,
                localizer.Localize(error.Code),
                error.Details,
                context.TraceIdentifier)).ConfigureAwait(false);
    }
}
