using System.Text.RegularExpressions;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// A phone number stored in E.164 form so the same subscriber is never recorded twice in
/// two different local formats.
/// </summary>
public sealed partial class PhoneNumber : ValueObject
{
    public const string InvalidPhoneNumber = "INVALID_PHONE_NUMBER";

    private PhoneNumber(string value) => Value = value;

    /// <summary>The number in E.164 form, for example <c>+250788123456</c>.</summary>
    public string Value { get; }

    public static PhoneNumber Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var compact = WhitespaceAndSeparators().Replace(value, string.Empty);

        return E164().IsMatch(compact)
            ? new PhoneNumber(compact)
            : throw new DomainException(Error.Validation(InvalidPhoneNumber));
    }

    public static bool TryCreate(string? value, out PhoneNumber? phoneNumber)
    {
        try
        {
            phoneNumber = value is null ? null : Create(value);
            return phoneNumber is not null;
        }
        catch (DomainException)
        {
            phoneNumber = null;
            return false;
        }
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"[\s\-\(\)\.]")]
    private static partial Regex WhitespaceAndSeparators();

    [GeneratedRegex(@"^\+[1-9]\d{7,14}$")]
    private static partial Regex E164();
}
