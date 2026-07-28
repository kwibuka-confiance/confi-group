using System.Reflection;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace ConfiOS.ArchitectureTests;

/// <summary>
/// Enforces the dependency rule from docs/03-architecture/02-clean-architecture.md.
/// These fail the build rather than a review, which is the point: layering erodes one
/// convenient reference at a time.
/// </summary>
public sealed class LayeringTests
{
    private static readonly Assembly[] DomainAssemblies =
    [
        typeof(Entity).Assembly,
        typeof(Modules.Identity.Domain.IdentityErrorCodes).Assembly,
    ];

    [Fact]
    public void Domain_does_not_depend_on_infrastructure()
    {
        var result = Types.InAssemblies(DomainAssemblies)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Npgsql",
                "StackExchange.Redis",
                "ConfiOS.BuildingBlocks.Infrastructure")
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Domain_does_not_depend_on_aspnetcore()
    {
        var result = Types.InAssemblies(DomainAssemblies)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_aspnetcore()
    {
        var result = Types.InAssemblies(
            [
                typeof(BuildingBlocks.Application.Context.ITenantContext).Assembly,
                typeof(Modules.Identity.Application.Abstractions.IPasswordHasher).Assembly,
            ])
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql",
                "ConfiOS.BuildingBlocks.Infrastructure")
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }
}
