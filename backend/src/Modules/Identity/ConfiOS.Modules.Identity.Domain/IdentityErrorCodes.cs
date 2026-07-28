namespace ConfiOS.Modules.Identity.Domain;

/// <summary>
/// Error codes owned by the Identity module. Each needs a matching resource entry in
/// every supported language.
/// </summary>
public static class IdentityErrorCodes
{
    public const string TenantSlugTaken = "TENANT_SLUG_TAKEN";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string BranchNotFound = "BRANCH_NOT_FOUND";
    public const string BranchCodeTaken = "BRANCH_CODE_TAKEN";
    public const string UserEmailTaken = "USER_EMAIL_TAKEN";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UserDeactivated = "USER_DEACTIVATED";
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string RoleNameTaken = "ROLE_NAME_TAKEN";
    public const string SystemRoleImmutable = "SYSTEM_ROLE_IMMUTABLE";
    public const string UnsupportedLanguage = "UNSUPPORTED_LANGUAGE";
    public const string LastOwnerCannotBeRemoved = "LAST_OWNER_CANNOT_BE_REMOVED";
}
