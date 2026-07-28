using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Infrastructure.Auditing;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.Modules.Identity.Api.Endpoints;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Application.Tenants.ProvisionTenant;
using ConfiOS.Modules.Identity.Application.Users.InviteUser;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using ConfiOS.Modules.Identity.Infrastructure.Repositories;
using ConfiOS.Modules.Identity.Infrastructure.Security;
using ConfiOS.Modules.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.Modules.Identity.Api;

/// <summary>
/// Wires up Identity. This is the only type the host sees; everything else in the module
/// is internal to it by convention.
/// </summary>
public sealed class IdentityModule : IModule
{
    public string Name => IdentityDbContext.SchemaName;

    /// <summary>
    /// Null: identity and tenant management is what every other feature is licensed
    /// against, so it can never itself be switched off.
    /// </summary>
    public string? FeatureKey => null;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<IdentityDbContext>((provider, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.MigrationsHistoryTable("__migrations", IdentityDbContext.SchemaName));

            options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IAuditLogger, AuditLogger<IdentityDbContext>>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<ICommandHandler<ProvisionTenantCommand, ProvisionTenantResult>, ProvisionTenantHandler>();
        services.AddScoped<ICommandHandler<InviteUserCommand, Guid>, InviteUserHandler>();

        services.AddScoped<BuildingBlocks.Application.Validation.IValidator<ProvisionTenantCommand>, ProvisionTenantValidator>();
        services.AddScoped<BuildingBlocks.Application.Validation.IValidator<InviteUserCommand>, InviteUserValidator>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapTenantEndpoints();
        endpoints.MapUserEndpoints();
    }
}
