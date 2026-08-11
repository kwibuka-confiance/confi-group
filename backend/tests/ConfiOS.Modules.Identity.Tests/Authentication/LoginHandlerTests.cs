using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Application.Authentication;
using ConfiOS.Modules.Identity.Application.Authentication.Login;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Identity.Tests.Authentication;

public sealed class LoginHandlerTests
{
    private const string Email = "owner@kwaconfi.rw";
    private const string Password = "Str0ngPass!23";
    private const string DepotHandle = "kwaconfi-depot";
    private const string CafeHandle = "kwaconfi-cafe";

    [Fact]
    public async Task One_matching_business_signs_in_without_choosing()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var user = CreateActiveUser(depot.TenantId);
        var unitOfWork = new FakeUnitOfWork();
        var handler = CreateHandler([depot], [user], passwordVerifies: true, unitOfWork: unitOfWork);

        var result = await handler.HandleAsync(new LoginCommand(Email, Password), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        var authenticated = result.Value.ShouldBeOfType<SignInOutcome.Authenticated>();
        authenticated.Session.BusinessName.ShouldBe("KwaConfi Depot");
        authenticated.Session.AccessToken.ShouldBe("signed-token");
        user.LastSignedInAt.ShouldNotBeNull();
        unitOfWork.SaveCount.ShouldBe(1);
    }

    [Fact]
    public async Task Several_matching_businesses_ask_the_caller_to_choose()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var cafe = CreateTenant("KwaConfi Cafe", CafeHandle);
        var handler = CreateHandler(
            [depot, cafe],
            [CreateActiveUser(depot.TenantId), CreateActiveUser(cafe.TenantId)],
            passwordVerifies: true);

        var result = await handler.HandleAsync(new LoginCommand(Email, Password), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        var choice = result.Value.ShouldBeOfType<SignInOutcome.ChoiceRequired>();
        choice.SelectionToken.ShouldBe("selection-token");
        choice.Businesses.Select(business => business.Slug)
            .ShouldBe([CafeHandle, DepotHandle]); // ordered by name
    }

    [Fact]
    public async Task A_handle_skips_the_choice_even_with_several_businesses()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var cafe = CreateTenant("KwaConfi Cafe", CafeHandle);
        var handler = CreateHandler(
            [depot, cafe],
            [CreateActiveUser(depot.TenantId), CreateActiveUser(cafe.TenantId)],
            passwordVerifies: true);

        var result = await handler.HandleAsync(
            new LoginCommand(Email, Password, CafeHandle),
            CancellationToken.None);

        var authenticated = result.Value.ShouldBeOfType<SignInOutcome.Authenticated>();
        authenticated.Session.BusinessName.ShouldBe("KwaConfi Cafe");
    }

    [Fact]
    public async Task A_wrong_password_is_rejected_as_unauthorized()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var handler = CreateHandler([depot], [CreateActiveUser(depot.TenantId)], passwordVerifies: false);

        var result = await handler.HandleAsync(new LoginCommand(Email, "nope"), CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
        result.Error.Type.ShouldBe(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task An_unknown_email_is_rejected_with_the_same_code()
    {
        var handler = CreateHandler([], [], passwordVerifies: true);

        var result = await handler.HandleAsync(
            new LoginCommand("ghost@example.com", Password),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
    }

    [Fact]
    public async Task A_deactivated_account_is_not_offered()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var user = CreateActiveUser(depot.TenantId);
        user.Deactivate();
        var handler = CreateHandler([depot], [user], passwordVerifies: true);

        var result = await handler.HandleAsync(new LoginCommand(Email, Password), CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
    }

    [Fact]
    public async Task An_unknown_handle_is_rejected_even_when_the_password_matches()
    {
        var depot = CreateTenant("KwaConfi Depot", DepotHandle);
        var handler = CreateHandler([depot], [CreateActiveUser(depot.TenantId)], passwordVerifies: true);

        var result = await handler.HandleAsync(
            new LoginCommand(Email, Password, "somebody-elses-shop"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(IdentityErrorCodes.InvalidCredentials);
    }

    private static Tenant CreateTenant(string name, string slug) =>
        Tenant.Register(name, slug, TenantSettings.Create("RW", "RWF", "en", "Africa/Kigali"));

    private static User CreateActiveUser(TenantId tenantId)
    {
        var user = User.Invite(tenantId, EmailAddress.Create(Email), "Confiance Owner", "HASH");
        user.Activate("HASH");
        return user;
    }

    private static LoginHandler CreateHandler(
        IReadOnlyList<Tenant> tenants,
        IReadOnlyList<User> users,
        bool passwordVerifies,
        FakeUnitOfWork? unitOfWork = null) =>
        new(
            new FakeTenantRepository(tenants),
            new FakeUserRepository(users),
            new FakePasswordHasher(passwordVerifies),
            new FakeSelectionTokens(),
            new SessionIssuer(
                new FakePermissionService(["identity.users.read"]),
                new FakeTokenGenerator(),
                new FixedClock(DateTimeOffset.UnixEpoch),
                unitOfWork ?? new FakeUnitOfWork()));

    private sealed class FakeTenantRepository(IReadOnlyList<Tenant> tenants) : ITenantRepository
    {
        public Task<Tenant?> GetAsync(TenantId tenantId, CancellationToken cancellationToken = default) =>
            Task.FromResult(tenants.FirstOrDefault(tenant => tenant.Id == tenantId.Value));

        public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
            Task.FromResult(tenants.FirstOrDefault(tenant => tenant.Slug == slug));

        public Task<IReadOnlyList<Tenant>> GetManyAsync(
            IReadOnlyCollection<Guid> tenantIds,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Tenant>>(
                tenants.Where(tenant => tenantIds.Contains(tenant.Id)).ToList());

        public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(Tenant tenant)
        {
        }
    }

    private sealed class FakeUserRepository(IReadOnlyList<User> users) : IUserRepository
    {
        public Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(users.FirstOrDefault(user => user.Id == userId.Value));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(users.Count == 0 ? null : users[0]);

        public Task<User?> GetForAuthenticationAsync(
            TenantId tenantId,
            string email,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(users.FirstOrDefault(user => user.TenantId == tenantId.Value));

        public Task<IReadOnlyList<User>> FindByEmailAcrossBusinessesAsync(
            string email,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<User>>(
                users.Where(user => user.Email.Value == email).ToList());

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

    private sealed class FakeSelectionTokens : IBusinessSelectionTokens
    {
        public BusinessSelectionToken Issue(string email, IReadOnlyCollection<Guid> tenantIds) =>
            new("selection-token", DateTimeOffset.UnixEpoch.AddMinutes(5));

        public BusinessSelectionPayload? Validate(string token) => null;
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
        public AccessToken Generate(AccessTokenClaims claims) =>
            new("signed-token", DateTimeOffset.UnixEpoch.AddHours(2));
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
