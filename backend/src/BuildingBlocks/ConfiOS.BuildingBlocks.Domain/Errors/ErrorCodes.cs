namespace ConfiOS.BuildingBlocks.Domain.Errors;

/// <summary>
/// Platform-wide error codes. Module-specific codes live with their module.
/// Every code here needs a matching entry in each ErrorMessages resource file.
/// </summary>
public static class ErrorCodes
{
    public const string Unknown = "UNKNOWN_ERROR";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string NotFound = "RESOURCE_NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string TenantContextMissing = "TENANT_CONTEXT_MISSING";
    public const string TenantMembershipRequired = "TENANT_MEMBERSHIP_REQUIRED";
    public const string BranchOutsideTenant = "BRANCH_OUTSIDE_TENANT";
    public const string FeatureNotLicensed = "FEATURE_NOT_LICENSED";
    public const string ConfirmationRequired = "CONFIRMATION_REQUIRED";
    public const string IdempotencyKeyRequired = "IDEMPOTENCY_KEY_REQUIRED";
    public const string IdempotencyKeyConflict = "IDEMPOTENCY_KEY_CONFLICT";
    public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";
    public const string CurrencyMismatch = "CURRENCY_MISMATCH";
    public const string UnsupportedCurrency = "UNSUPPORTED_CURRENCY";
    public const string NegativeAmount = "NEGATIVE_AMOUNT";
}
