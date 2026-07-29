using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Authorization;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Application.Validation;
using ConfiOS.BuildingBlocks.Infrastructure.Auditing;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using ConfiOS.Modules.Catalog.Api.Endpoints;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Application.Products.CreateProduct;
using ConfiOS.Modules.Catalog.Application.Products.GetProducts;
using ConfiOS.Modules.Catalog.Domain.Authorization;
using ConfiOS.Modules.Catalog.Infrastructure.Persistence;
using ConfiOS.Modules.Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.Modules.Catalog.Api;

/// <summary>
/// Products, categories, brands and units. This first slice covers products.
/// </summary>
public sealed class CatalogModule : IModule
{
    public string Name => CatalogDbContext.SchemaName;

    public string? FeatureKey => "catalog";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<CatalogDbContext>((provider, options) =>
        {
            options.UseConfiOsPostgres(
                configuration.GetConnectionString("Postgres"),
                CatalogDbContext.SchemaName);

            options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<ICatalogUnitOfWork>(provider => provider.GetRequiredService<CatalogDbContext>());
        services.AddScoped<IAuditLogger, AuditLogger<CatalogDbContext>>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<ICommandHandler<CreateProductCommand, Guid>, CreateProductHandler>();
        services.AddScoped<IQueryHandler<GetProductsQuery, IReadOnlyList<ProductSummary>>, GetProductsHandler>();
        services.AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>();

        services.AddSingleton(new ModulePermissions(CatalogPermissions.All));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapProductEndpoints();
    }
}
