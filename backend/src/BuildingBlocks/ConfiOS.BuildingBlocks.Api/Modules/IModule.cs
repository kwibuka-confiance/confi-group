using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.BuildingBlocks.Api.Modules;

/// <summary>
/// One module of the modular monolith. The host discovers implementations and calls them;
/// it never references a module's internal types.
/// </summary>
/// <remarks>
/// This is the seam that keeps extraction cheap. A module that only ever appears to the
/// host through this interface can be lifted into its own service without the host
/// changing (docs/03-architecture/03-modular-monolith.md).
/// </remarks>
public interface IModule
{
    /// <summary>Module name, matching its database schema, for example <c>identity</c>.</summary>
    string Name { get; }

    /// <summary>
    /// Feature key checked against the tenant's subscription before the module's endpoints
    /// respond (MT-006). Null for modules that are always available.
    /// </summary>
    string? FeatureKey { get; }

    /// <summary>Registers the module's services, database context and handlers.</summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>Maps the module's HTTP endpoints under the versioned API root.</summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
