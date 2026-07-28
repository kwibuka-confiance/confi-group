using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Identity.Application.Users.GetUsers;

/// <summary>Lists users in the current tenant.</summary>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Items per page, capped by the handler.</param>
/// <param name="Search">Optional name or email fragment.</param>
public sealed record GetUsersQuery(int Page = 1, int PageSize = 25, string? Search = null)
    : IQuery<UserListPage>;

/// <summary>One page of users.</summary>
/// <param name="Items">Users on this page.</param>
/// <param name="TotalCount">Total users matching the query.</param>
public sealed record UserListPage(IReadOnlyList<UserSummary> Items, long TotalCount);

/// <summary>
/// A user as shown in lists. Deliberately does not carry the password hash or any other
/// credential material.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Email">Email address.</param>
/// <param name="FullName">Person's name.</param>
/// <param name="Status">Lifecycle state, as a stable string.</param>
/// <param name="Roles">Role names granted.</param>
/// <param name="LastSignedInAt">Last successful sign-in, if any.</param>
public sealed record UserSummary(
    Guid Id,
    string Email,
    string FullName,
    string Status,
    IReadOnlyList<string> Roles,
    DateTimeOffset? LastSignedInAt);
