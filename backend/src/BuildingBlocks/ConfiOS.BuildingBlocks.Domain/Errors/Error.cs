namespace ConfiOS.BuildingBlocks.Domain.Errors;

/// <summary>
/// A failure with a stable machine-readable code, per rule 8 in CLAUDE.md and the error
/// contract in docs/03-architecture/05-api-standards.md.
/// </summary>
/// <remarks>
/// <paramref name="Code"/> is the contract with clients and never changes once released.
/// No human-readable message is carried here: messages are resolved from resources at the
/// edge using the request's Accept-Language, so the same code renders in en, rw or fr.
/// </remarks>
/// <param name="Code">Stable machine-readable code, for example <c>INSUFFICIENT_STOCK</c>.</param>
/// <param name="Type">How the caller should interpret the failure.</param>
/// <param name="Details">Structured context for the client, safe to display.</param>
public sealed record Error(
    string Code,
    ErrorType Type = ErrorType.Failure,
    IReadOnlyDictionary<string, object?>? Details = null)
{
    public static readonly Error None = new(string.Empty);

    public static Error Validation(string code, IReadOnlyDictionary<string, object?>? details = null)
        => new(code, ErrorType.Validation, details);

    public static Error NotFound(string code, IReadOnlyDictionary<string, object?>? details = null)
        => new(code, ErrorType.NotFound, details);

    public static Error Conflict(string code, IReadOnlyDictionary<string, object?>? details = null)
        => new(code, ErrorType.Conflict, details);

    public static Error Forbidden(string code, IReadOnlyDictionary<string, object?>? details = null)
        => new(code, ErrorType.Forbidden, details);

    public static Error Unauthorized(string code, IReadOnlyDictionary<string, object?>? details = null)
        => new(code, ErrorType.Unauthorized, details);
}

/// <summary>Maps a failure onto a transport-level category.</summary>
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4,
    Unauthorized = 5,
}
