using Microsoft.Extensions.Localization;

namespace ConfiOS.BuildingBlocks.Api.Localization;

/// <summary>
/// Turns a stable error code into a message in the request's language.
/// Rule 3 in CLAUDE.md: no visible string is hardcoded.
/// </summary>
public interface IErrorMessageLocalizer
{
    string Localize(string errorCode);
}

/// <summary>Resource-file backed localizer.</summary>
/// <param name="localizer">Resource lookup for <see cref="ErrorMessages"/>.</param>
public sealed class ErrorMessageLocalizer(IStringLocalizer<ErrorMessages> localizer) : IErrorMessageLocalizer
{
    public string Localize(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
        {
            return string.Empty;
        }

        var localized = localizer[errorCode];

        // A missing resource returns the key itself. Falling back to the generic message
        // keeps a raw code such as INSUFFICIENT_STOCK off the user's screen, while the
        // response still carries the code for the client to act on.
        return localized.ResourceNotFound
            ? localizer[Domain.Errors.ErrorCodes.Unknown]
            : localized.Value;
    }
}
