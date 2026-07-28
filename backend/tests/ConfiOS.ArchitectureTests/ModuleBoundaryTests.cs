using System.Reflection;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace ConfiOS.ArchitectureTests;

/// <summary>
/// Enforces the module rules from docs/03-architecture/03-modular-monolith.md: a module
/// talks to another module through contracts and events, never by referencing its types.
/// </summary>
public sealed class ModuleBoundaryTests
{
    private static readonly string[] ModuleNames = ["Identity", "Catalog", "Inventory", "Sales"];

    [Theory]
    [InlineData("Identity")]
    [InlineData("Catalog")]
    [InlineData("Inventory")]
    [InlineData("Sales")]
    public void A_module_does_not_reference_another_modules_internals(string moduleName)
    {
        var assemblies = LoadModuleAssemblies(moduleName);

        if (assemblies.Length == 0)
        {
            // The module has no compiled types yet. Nothing to check.
            return;
        }

        var forbidden = ModuleNames
            .Where(other => !string.Equals(other, moduleName, StringComparison.Ordinal))
            .Select(other => $"ConfiOS.Modules.{other}")
            .ToArray();

        var result = Types.InAssemblies(assemblies)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    private static Assembly[] LoadModuleAssemblies(string moduleName)
    {
        var prefix = $"ConfiOS.Modules.{moduleName}.";

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.GetName().Name?.StartsWith(prefix, StringComparison.Ordinal) == true)
            .ToArray();
    }
}
