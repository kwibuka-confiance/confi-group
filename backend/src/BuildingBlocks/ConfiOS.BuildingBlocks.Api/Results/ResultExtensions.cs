using ConfiOS.BuildingBlocks.Api.Contracts;
using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Domain.Errors;
using Microsoft.AspNetCore.Http;

namespace ConfiOS.BuildingBlocks.Api.Results;

/// <summary>
/// Maps an application <see cref="Result"/> onto an HTTP response in the standard
/// envelope. This is the only place that decides status codes, so endpoints stay free of
/// transport branching.
/// </summary>
public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(
        this Result<T> result,
        HttpContext httpContext,
        IErrorMessageLocalizer localizer,
        object? meta = null,
        int successStatusCode = StatusCodes.Status200OK)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(httpContext);

        var traceId = httpContext.TraceIdentifier;

        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Json(
                new ApiResponse<T>(result.Value, meta, traceId),
                statusCode: successStatusCode)
            : ToProblem(result.Error, traceId, localizer);
    }

    public static IResult ToHttpResult(
        this Result result,
        HttpContext httpContext,
        IErrorMessageLocalizer localizer,
        int successStatusCode = StatusCodes.Status204NoContent)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(httpContext);

        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.StatusCode(successStatusCode)
            : ToProblem(result.Error, httpContext.TraceIdentifier, localizer);
    }

    public static IResult ToProblem(Error error, string traceId, IErrorMessageLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(localizer);

        return Microsoft.AspNetCore.Http.Results.Json(
            new ApiError(error.Code, localizer.Localize(error.Code), error.Details, traceId),
            statusCode: ToStatusCode(error.Type));
    }

    public static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError,
    };
}
