using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Application.Authentication.Login;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Identity.Tests.Authentication;

public sealed class LoginHandlerTests
{
    private const string Handle = "kwaconfi-depot";
    private const string Email = "owner@kwaconfi.rw";
    private const string Password = "Str0ngPass!23";

    [Fact]
    public async Task Valid_credentials_issue_a_token_and_record_the_sign_in()
    {
        var tenant = CreateTenant();
        var user = CreateActiveUser(tenant.TenantId);
        var tokenGenerator = new FakeTokenGenerator();
        var unitOfWork = new FakeUnitOfWork();
        var handler = CreateHandler(tenant, user, passwordVerifies: true, tokenGenerator, unitOfWork);

        var result = await handler.HandleAsync(
            new LoginCommand(Handle, Email, Password),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldBe("signed-token");
        result.Value.BusinessName.ShouldBe("KwaConfi Depot");
        result.Value.Permissions.ShouldContain("identity.users.read");
        tokenGenerator.Captured!.TenantId.ShouldBe(tenant.TenantId);
        user.LastSignedInAt.ShouldNotBeNull();
        unitOfWork.SaveCount.ShouldBe(1);
    }

    [Fact]
    public async Task A_wrong_password_is_rejected_as_unauthorized()
    {
        var tenant = CreateTenant();
        var user = CreateActiveUser(tenant.TenantId);
        var handler = CreateHandler(tenant, user, passwordVerifies: false);

        var result = await handler.HandleAsync(
            new LoginCommand(Handle, Email, "nope"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
        result.Error.Type.ShouldBe(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task An_unknown_business_is_rejected_with_the_same_code()
    {
        var handler = CreateHandler(tenant: null, user: null, passwordVerifies: true);

        var result = await handler.HandleAsync(
            new LoginCommand("ghost-business", Email, Password),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
    }

    [Fact]
    public async Task A_deactivated_user_cannot_sign_in()
    {
        var tenant = CreateTenant();
        var user = CreateActiveUser(tenant.TenantId);
        user.Deactivate();
        var handler = CreateHandler(tenant, user, passwordVerifies: true);

        var result = await handler.HandleAsync(
            new LoginCommand(Handle, Email, Password),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
    }

    private static Tenant CreateTenant() =>
        Tenant.Register(
            "KwaConfi Depot",
            Handle,
            TenantSettings.Create("RW", "RWF", "en", "Africa/Kigali"));

    private static User CreateActiveUser(TenantId tenantId)
    {
        var user = User.Invite(tenantId, EmailAddress.Create(Email), "Confiance Owner", "HASH");
        user.Activate("HASH");
        return user;
    }

    private static LoginHandler CreateHandler(
        Tenant? tenant,
        User? user,
        bool passwordVerifies,
        FakeTokenGenerator? tokenGenerator = null,
        FakeUnitOfWork? unitOfWork = null) =>
        new(
            new FakeTenantRepository(tenant),
            new FakeUserRepository(user),
            new FakePasswordHasher(passwordVerifies),
            new FakePermissionService(["identity.users.read"]),
            tokenGenerator ?? new FakeTokenGenerator(),
            new FixedClock(DateTimeOffset.UnixEpoch),
            unitOfWork ?? new FakeUnitOfWork());

    private sealed class FakeTenantRepository(Tenant? tenant) : ITenantRepository
    {
        public Task<Tenant?> GetAsync(TenantId tenantId, CancellationToken cancellationToken = default) =>
            Task.FromResult(tenant);

        public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
            Task.FromResult(tenant is not null && tenant.Slug == slug ? tenant : null);

        public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(Tenant tenant)
        {
        }
    }

    private sealed class FakeUserRepository(User? user) : IUserRepository
    {
        public Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(user);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(user);

        public Task<User?> GetForAuthenticationAsync(
            TenantId tenantId,
            string email,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(user);

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(User user)
        {
        }
    }

    private sealed class FakePasswordHasher(bool verifies) : IPasswordHasher
    {
        public string Hash(string password) => "HASH";

        public bool Verify(string password, string hash) => verifies;
    }

    private sealed class FakePermissionService(IReadOnlyList<string> permissions) : IPermissionService
    {
        public Task<bool> HasPermissionAsync(
            UserId userId,
            TenantId tenantId,
            string permission,
            CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<bool> HasBranchAccessAsync(
            UserId userId,
            TenantId tenantId,
            BranchId branchId,
            CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<IReadOnlyList<string>> GetPermissionsAsync(
            UserId userId,
            TenantId tenantId,
            CancellationToken cancellationToken = default) => Task.FromResult(permissions);
    }

    private sealed class FakeTokenGenerator : IAccessTokenGenerator
    {
        public AccessTokenClaims? Captured { get; private set; }

        public AccessToken Generate(AccessTokenClaims claims)
        {
            Captured = claims;
            return new AccessToken("signed-token", DateTimeOffset.UnixEpoch.AddHours(2));
        }
    }

    private sealed class FixedClock(DateTimeOffset now) : IClock
    {
        public DateTimeOffset UtcNow => now;
    }

    private sealed class FakeUnitOfWork : IIdentityUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }

        public Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IAsyncDisposable>(new NoopTransaction());

        private sealed class NoopTransaction : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
