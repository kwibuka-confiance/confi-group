namespace ConfiOS.Modules.Identity.Domain.Authorization;

/// <summary>
/// Every permission the platform recognises, named <c>module.resource.action</c>.
/// </summary>
/// <remarks>
/// Kept as constants rather than an enum so modules can add their own without a
/// coordinated change here, and so a permission stored in the database is readable
/// without a lookup table.
/// </remarks>
public static class Permissions
{
    public static class Tenants
    {
        public const string Read = "identity.tenants.read";
        public const string Update = "identity.tenants.update";
        public const string Suspend = "identity.tenants.suspend";
    }

    public static class Branches
    {
        public const string Read = "identity.branches.read";
        public const string Create = "identity.branches.create";
        public const string Update = "identity.branches.update";
        public const string Archive = "identity.branches.archive";
    }

    public static class Users
    {
        public const string Read = "identity.users.read";
        public const string Invite = "identity.users.invite";
        public const string Update = "identity.users.update";
        public const string Deactivate = "identity.users.deactivate";
        public const string AssignRoles = "identity.users.assign-roles";
    }

    public static class Roles
    {
        public const string Read = "identity.roles.read";
        public const string Create = "identity.roles.create";
        public const string Update = "identity.roles.update";
        public const string Delete = "identity.roles.delete";
    }

    /// <summary>Every permission defined by this module.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Tenants.Read, Tenants.Update, Tenants.Suspend,
        Branches.Read, Branches.Create, Branches.Update, Branches.Archive,
        Users.Read, Users.Invite, Users.Update, Users.Deactivate, Users.AssignRoles,
        Roles.Read, Roles.Create, Roles.Update, Roles.Delete,
    ];
}

/// <summary>
/// Roles created automatically for every new tenant. A tenant may add its own, but these
/// always exist so a freshly provisioned business is usable immediately.
/// </summary>
public static class SystemRoles
{
    /// <summary>Full control, including billing and tenant settings. Cannot be removed.</summary>
    public const string Owner = "owner";

    /// <summary>Day-to-day management of a branch.</summary>
    public const string BranchManager = "branch-manager";

    /// <summary>Sells at the counter.</summary>
    public const string Cashier = "cashier";

    /// <summary>Receives, transfers and counts stock.</summary>
    public const string StoreKeeper = "store-keeper";

    public static IReadOnlyList<string> All { get; } = [Owner, BranchManager, Cashier, StoreKeeper];
}
