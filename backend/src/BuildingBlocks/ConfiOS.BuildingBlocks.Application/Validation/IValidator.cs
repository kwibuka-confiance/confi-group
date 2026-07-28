using ConfiOS.BuildingBlocks.Domain.Errors;

namespace ConfiOS.BuildingBlocks.Application.Validation;

/// <summary>
/// Application-level validation for a command or query: shape, ranges and required
/// fields. Business invariants stay in the domain, so this never replaces them
/// (docs/04-development/04-dotnet-standards.md).
/// </summary>
/// <typeparam name="TRequest">Request validated.</typeparam>
public interface IValidator<in TRequest>
{
    ValidationResult Validate(TRequest request);
}

/// <summary>Outcome of validating a request.</summary>
public sealed class ValidationResult
{
    private readonly List<ValidationFailure> _failures = [];

    public bool IsValid => _failures.Count == 0;

    public IReadOnlyList<ValidationFailure> Failures => _failures;

    public static ValidationResult Valid() => new();

    public ValidationResult Add(string propertyName, string code)
    {
        _failures.Add(new ValidationFailure(propertyName, code));
        return this;
    }

    /// <summary>Adds a failure only when <paramref name="condition"/> holds.</summary>
    public ValidationResult AddIf(bool condition, string propertyName, string code) =>
        condition ? Add(propertyName, code) : this;

    /// <summary>
    /// Collapses the failures into a single error. The per-field codes travel in details
    /// so a client can highlight the offending inputs.
    /// </summary>
    public Error ToError() => Error.Validation(
        ErrorCodes.ValidationFailed,
        new Dictionary<string, object?>
        {
            ["failures"] = _failures
                .GroupBy(failure => failure.PropertyName, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => (object?)group.Select(failure => failure.Code).ToArray(),
                    StringComparer.Ordinal),
        });
}

/// <summary>A single validation failure.</summary>
/// <param name="PropertyName">Request property at fault, in camelCase to match the wire format.</param>
/// <param name="Code">Stable machine-readable code, for example <c>REQUIRED</c>.</param>
public sealed record ValidationFailure(string PropertyName, string Code);

/// <summary>Codes shared by validators across modules.</summary>
public static class ValidationCodes
{
    public const string Required = "REQUIRED";
    public const string TooLong = "TOO_LONG";
    public const string TooShort = "TOO_SHORT";
    public const string OutOfRange = "OUT_OF_RANGE";
    public const string InvalidFormat = "INVALID_FORMAT";
    public const string MustBePositive = "MUST_BE_POSITIVE";
    public const string NotSupported = "NOT_SUPPORTED";
}
