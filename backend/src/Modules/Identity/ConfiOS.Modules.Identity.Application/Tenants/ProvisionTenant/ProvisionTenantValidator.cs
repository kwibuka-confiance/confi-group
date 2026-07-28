using System.Text.RegularExpressions;
using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Identity.Application.Tenants.ProvisionTenant;

/// <summary>Shape and range checks for <see cref="ProvisionTenantCommand"/>.</summary>
public sealed partial class ProvisionTenantValidator : IValidator<ProvisionTenantCommand>
{
    /// <summary>
    /// Minimum password length. Length is the single most useful requirement; composition
    /// rules mostly push people towards predictable substitutions.
    /// </summary>
    public const int MinimumPasswordLength = 12;

    public ValidationResult Validate(ProvisionTenantCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidationResult.Valid();

        result.AddIf(string.IsNullOrWhiteSpace(request.Name), nameof(request.Name), ValidationCodes.Required);
        result.AddIf(request.Name?.Length > 200, nameof(request.Name), ValidationCodes.TooLong);

        result.AddIf(string.IsNullOrWhiteSpace(request.Slug), nameof(request.Slug), ValidationCodes.Required);
        result.AddIf(
            !string.IsNullOrWhiteSpace(request.Slug) && !SlugPattern().IsMatch(request.Slug),
            nameof(request.Slug),
            ValidationCodes.InvalidFormat);

        result.AddIf(
            request.CountryCode?.Length != 2,
            nameof(request.CountryCode),
            ValidationCodes.InvalidFormat);

        result.AddIf(
            request.CurrencyCode?.Length != 3,
            nameof(request.CurrencyCode),
            ValidationCodes.InvalidFormat);

        result.AddIf(
            string.IsNullOrWhiteSpace(request.OwnerEmail),
            nameof(request.OwnerEmail),
            ValidationCodes.Required);

        result.AddIf(
            string.IsNullOrWhiteSpace(request.OwnerFullName),
            nameof(request.OwnerFullName),
            ValidationCodes.Required);

        result.AddIf(
            string.IsNullOrEmpty(request.OwnerPassword) || request.OwnerPassword.Length < MinimumPasswordLength,
            nameof(request.OwnerPassword),
            ValidationCodes.TooShort);

        result.AddIf(
            string.IsNullOrWhiteSpace(request.FirstBranchName),
            nameof(request.FirstBranchName),
            ValidationCodes.Required);

        return result;
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();
}
