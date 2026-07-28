using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConfiOS.BuildingBlocks.Api;

/// <summary>Registers the shared transport-level services and middleware.</summary>
public static class ApiServiceCollectionExtensions
{
    /// <summary>Languages the MVP ships with (CLAUDE.md).</summary>
    public static readonly string[] SupportedCultures = ["en", "rw", "fr"];

    /// <summary>Adds localization and the error message localizer.</summary>
    public static IServiceCollection AddBuildingBlocksApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLocalization();
        services.TryAddScoped<IErrorMessageLocalizer, ErrorMessageLocalizer>();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture(SupportedCultures[0]);
            options.AddSupportedCultures(SupportedCultures);
            options.AddSupportedUICultures(SupportedCultures);
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        return services;
    }

    /// <summary>
    /// Inserts the shared middleware. Order matters: exceptions are caught outermost,
    /// language is negotiated before any message is produced, and tenant context is
    /// resolved after authentication has run.
    /// </summary>
    public static IApplicationBuilder UseBuildingBlocksApi(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseRequestLocalization();

        return app;
    }

    /// <summary>Resolves tenant context. Call after UseAuthentication and UseAuthorization.</summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<TenantResolutionMiddleware>();
    }
}
