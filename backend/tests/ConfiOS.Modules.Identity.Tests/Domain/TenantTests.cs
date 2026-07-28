using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Events;
using ConfiOS.Modules.Identity.Domain.Tenants;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Identity.Tests.Domain;

public sealed class TenantTests
{
    [Fact]
    public void Registering_raises_TenantCreated()
    {
        var tenant = Tenant.Register("KwaConfi Depot", "kwaconfi-depot", TenantSettings.Default());

        tenant.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<TenantCreated>();
        tenant.Status.ShouldBe(TenantStatus.Active);
    }

    [Fact]
    public void Branch_codes_are_unique_within_the_tenant()
    {
        var tenant = Tenant.Register("KwaConfi Depot", "kwaconfi-depot", TenantSettings.Default());
        tenant.AddBranch("Head Office", "HQ", address: null);

        var exception = Should.Throw<DomainException>(
            () => tenant.AddBranch("Second Office", "hq", address: null));

        exception.Error.Code.ShouldBe(IdentityErrorCodes.BranchCodeTaken);
    }

    [Fact]
    public void A_suspended_tenant_cannot_be_changed()
    {
        var tenant = Tenant.Register("KwaConfi Depot", "kwaconfi-depot", TenantSettings.Default());
        tenant.Suspend("Non-payment");

        var exception = Should.Throw<DomainException>(
            () => tenant.AddBranch("Musanze", "MUS", address: null));

        exception.Error.Code.ShouldBe(IdentityErrorCodes.TenantSuspended);
    }

    [Fact]
    public void Suspending_twice_raises_one_event()
    {
        var tenant = Tenant.Register("KwaConfi Depot", "kwaconfi-depot", TenantSettings.Default());
        tenant.ClearDomainEvents();

        tenant.Suspend("Non-payment");
        tenant.Suspend("Non-payment");

        tenant.DomainEvents.Count(domainEvent => domainEvent is TenantSuspended).ShouldBe(1);
    }

    [Fact]
    public void An_unsupported_language_is_rejected()
    {
        var exception = Should.Throw<DomainException>(
            () => TenantSettings.Create("RW", "RWF", "sw", "Africa/Kigali"));

        exception.Error.Code.ShouldBe(IdentityErrorCodes.UnsupportedLanguage);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("rw")]
    [InlineData("fr")]
    public void The_three_mvp_languages_are_accepted(string language)
    {
        var settings = TenantSettings.Create("RW", "RWF", language, "Africa/Kigali");

        settings.DefaultLanguage.ShouldBe(language);
    }
}
