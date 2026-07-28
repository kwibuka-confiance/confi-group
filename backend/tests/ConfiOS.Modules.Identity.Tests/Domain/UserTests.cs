using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Events;
using ConfiOS.Modules.Identity.Domain.Users;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Identity.Tests.Domain;

public sealed class UserTests
{
    private static readonly TenantId Tenant = TenantId.New();

    [Fact]
    public void An_invited_user_is_not_yet_active()
    {
        var user = Invite();

        user.Status.ShouldBe(UserStatus.Invited);
        user.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<UserInvited>();
    }

    [Fact]
    public void An_invited_user_cannot_sign_in_before_accepting()
    {
        var user = Invite();

        var exception = Should.Throw<DomainException>(
            () => user.RecordSignIn(DateTimeOffset.UtcNow));

        exception.Error.Code.ShouldBe(IdentityErrorCodes.UserDeactivated);
    }

    [Fact]
    public void A_deactivated_user_cannot_change_their_password()
    {
        var user = Invite();
        user.Activate("hash");
        user.Deactivate();

        Should.Throw<DomainException>(() => user.ChangePassword("new-hash"));
    }

    [Fact]
    public void Assigning_the_same_role_twice_is_idempotent()
    {
        var user = Invite();
        var roleId = Guid.NewGuid();

        user.AssignRole(roleId);
        user.AssignRole(roleId);

        user.Roles.Count.ShouldBe(1);
    }

    [Fact]
    public void Changing_language_raises_LanguageChanged()
    {
        var user = Invite();
        user.ClearDomainEvents();

        user.ChangeLanguage("rw");

        user.PreferredLanguage.ShouldBe("rw");
        user.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<LanguageChanged>();
    }

    [Fact]
    public void Branch_access_is_recorded_once_per_branch()
    {
        var user = Invite();
        var branchId = BranchId.New();

        user.GrantBranchAccess(branchId);
        user.GrantBranchAccess(branchId);
        user.Branches.Count.ShouldBe(1);

        user.RevokeBranchAccess(branchId);
        user.Branches.ShouldBeEmpty();
    }

    private static User Invite() =>
        User.Invite(Tenant, EmailAddress.Create("owner@kwaconfi.rw"), "Confiance K", "hash");
}
