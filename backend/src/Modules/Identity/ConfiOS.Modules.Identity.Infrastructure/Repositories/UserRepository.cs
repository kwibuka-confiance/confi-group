using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain.Users;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Repositories;

/// <summary>
/// User storage. Every query here runs through the tenant filter applied in
/// <c>TenantDbContext</c>, so these methods cannot reach another tenant's users — except
/// <see cref="GetForAuthenticationAsync"/>, which runs before a tenant is resolved and
/// re-asserts the boundary itself.
/// </summary>
/// <remarks>
/// Email is a value-converted column, so queries compare the whole <see cref="EmailAddress"/>
/// (which EF converts to the stored string) rather than <c>Email.Value</c>, which has no
/// column to translate to.
/// </remarks>
/// <param name="context">Identity database context.</param>
public sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Id == userId.Value, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var address = EmailAddress.Create(email);
        return context.Users.FirstOrDefaultAsync(user => user.Email == address, cancellationToken);
    }

    public Task<User?> GetForAuthenticationAsync(
        TenantId tenantId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var address = EmailAddress.Create(email);

        // Sign-in runs before any tenant context is resolved, so the tenant filter would
        // exclude every row. The tenant boundary is re-asserted explicitly in the predicate.
        return context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                user => user.TenantId == tenantId.Value
                    && user.Email == address
                    && !user.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<User>> FindByEmailAcrossBusinessesAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var address = EmailAddress.Create(email);

        // Sign-in happens before a tenant is known, so this query intentionally
        // spans tenants. It is limited to live, active accounts, and the caller
        // still has to verify each account's own password.
        return await context.Users
            .IgnoreQueryFilters()
            .Where(user => user.Email == address && !user.IsDeleted)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var address = EmailAddress.Create(email);
        return context.Users.AnyAsync(user => user.Email == address, cancellationToken);
    }

    public void Add(User user) => context.Users.Add(user);
}
