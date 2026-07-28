namespace ConfiOS.BuildingBlocks.Api.Contracts;

/// <summary>
/// Success envelope from docs/03-architecture/05-api-standards.md.
/// </summary>
/// <typeparam name="T">Payload type.</typeparam>
/// <param name="Data">The payload.</param>
/// <param name="Meta">Pagination and other envelope metadata.</param>
/// <param name="TraceId">Correlates the response with logs and traces.</param>
public sealed record ApiResponse<T>(T Data, object? Meta, string TraceId);

/// <summary>
/// Error envelope. The code is stable across locales; the message is localised for
/// display only and must never be parsed by clients.
/// </summary>
/// <param name="Code">Stable machine-readable code, for example <c>INSUFFICIENT_STOCK</c>.</param>
/// <param name="Message">Localised, human-readable message.</param>
/// <param name="Details">Structured context, such as which fields failed validation.</param>
/// <param name="TraceId">Correlates the response with logs and traces.</param>
public sealed record ApiError(
    string Code,
    string Message,
    IReadOnlyDictionary<string, object?>? Details,
    string TraceId);

/// <summary>Pagination metadata returned in <c>meta</c> for list endpoints.</summary>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Items per page.</param>
/// <param name="TotalCount">Total items matching the query.</param>
public sealed record PageMeta(int Page, int PageSize, long TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
