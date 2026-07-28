using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain.Users;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Repositories;

/// <summary>
/// User storage. Every query here runs through the tenant filter applied in
/// <c>TenantDbContext</c>, so these methods cannot reach another tenant's users.
/// </summary>
/// <param name="context">Identity database context.</param>
public sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Id == userId.Value, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Email.Value == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(user => user.Email.Value == email, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
}
