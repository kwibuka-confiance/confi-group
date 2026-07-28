using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Identity.Application.Tenants.ProvisionTenant;

/// <summary>
/// Creates a business, its first branch, its system roles and its owner account.
/// </summary>
/// <remarks>
/// This is the one command that runs without a resolved tenant: it is what creates one.
/// It is therefore restricted to platform administration and self-service sign-up.
/// </remarks>
/// <param name="Name">Trading name of the business.</param>
/// <param name="Slug">URL-safe handle, unique across the platform.</param>
/// <param name="CountryCode">ISO 3166-1 alpha-2 country code.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
/// <param name="DefaultLanguage">BCP 47 default language.</param>
/// <param name="TimeZoneId">IANA time zone.</param>
/// <param name="OwnerEmail">Email of the first owner account.</param>
/// <param name="OwnerFullName">Name of the first owner.</param>
/// <param name="OwnerPassword">Initial password. Hashed immediately and never stored or logged.</param>
/// <param name="FirstBranchName">Name of the branch created with the business.</param>
public sealed record ProvisionTenantCommand(
    string Name,
    string Slug,
    string CountryCode,
    string CurrencyCode,
    string DefaultLanguage,
    string TimeZoneId,
    string OwnerEmail,
    string OwnerFullName,
    string OwnerPassword,
    string FirstBranchName) : ICommand<ProvisionTenantResult>;

/// <summary>Identifiers of the records created.</summary>
/// <param name="TenantId">The new tenant.</param>
/// <param name="BranchId">The first branch.</param>
/// <param name="OwnerUserId">The owner account.</param>
public sealed record ProvisionTenantResult(Guid TenantId, Guid BranchId, Guid OwnerUserId);
