using ConfiOS.BuildingBlocks.Api.Contracts;
using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Middleware;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Catalog.Api.Contracts;
using ConfiOS.Modules.Catalog.Application.Products.CreateProduct;
using ConfiOS.Modules.Catalog.Application.Products.GetProducts;
using ConfiOS.Modules.Catalog.Domain.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ConfiOS.Modules.Catalog.Api.Endpoints;

/// <summary>Product endpoints for the current tenant.</summary>
public static class ProductEndpoints
{
    /// <summary>Maps the product routes under <c>/api/v1/products</c>.</summary>
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var group = endpoints
            .MapGroup("/api/v1/products")
            .WithTags("Products")
            .RequireAuthorization()
            .AddEndpointFilter<RequireTenantFilter>();

        group.MapPost("/", CreateAsync)
            .RequireAuthorization(CatalogPermissions.Products.Create)
            .WithName("CreateProduct")
            .WithSummary("Adds a product to the catalog.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", ListAsync)
            .RequireAuthorization(CatalogPermissions.Products.Read)
            .WithName("GetProducts")
            .WithSummary("Lists the products in the catalog.")
            .Produces<ApiResponse<IReadOnlyList<ProductSummary>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateProductRequest request,
        IDispatcher dispatcher,
        IErrorMessageLocalizer localizer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await dispatcher.SendAsync(
            new CreateProductCommand(
                request.Name,
                request.Sku,
                request.PriceAmount,
                request.CurrencyCode,
                request.BaseUnitCode,
                request.Description,
                request.CostAmount,
                request.TaxClass,
                request.DepositAmount,
                request.Packagings
                    ?.Select(packaging => new PackagingInput(
                        packaging.UnitCode,
                        packaging.QuantityInBaseUnit,
                        packaging.SellingPriceAmount,
                        packaging.CostAmount,
                        packaging.Barcode))
                    .ToList()),
            cancellationToken).ConfigureAwait(false);

        return result.ToHttpResult(httpContext, localizer, successStatusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> ListAsync(
        IDispatcher dispatcher,
        IErrorMessageLocalizer localizer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher
            .QueryAsync(new GetProductsQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToHttpResult(httpContext, localizer);
    }
}
