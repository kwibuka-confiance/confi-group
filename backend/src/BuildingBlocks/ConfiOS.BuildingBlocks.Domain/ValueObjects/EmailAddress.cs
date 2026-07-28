using System.Text.RegularExpressions;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// An email address, lower-cased so lookups and uniqueness checks behave consistently.
/// </summary>
public sealed partial class EmailAddress : ValueObject
{
    public const string InvalidEmailAddress = "INVALID_EMAIL_ADDRESS";

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalised = value.Trim().ToLowerInvariant();

        return normalised.Length <= 320 && Pattern().IsMatch(normalised)
            ? new EmailAddress(normalised)
            : throw new DomainException(Error.Validation(InvalidEmailAddress));
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s\.]+(\.[^@\s\.]+)+$")]
    private static partial Regex Pattern();
}
