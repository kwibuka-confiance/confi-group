using ConfiOS.BuildingBlocks.Api.Contracts;
using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Identity.Api.Contracts;
using ConfiOS.Modules.Identity.Application.Tenants.ProvisionTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ConfiOS.Modules.Identity.Api.Endpoints;

/// <summary>Tenant provisioning and administration endpoints.</summary>
public static class TenantEndpoints
{
    /// <summary>Maps the tenant routes under <c>/api/v1/tenants</c>.</summary>
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var group = endpoints
            .MapGroup("/api/v1/tenants")
            .WithTags("Tenants");

        group.MapPost("/", ProvisionAsync)
            .AllowAnonymous()
            .WithName("ProvisionTenant")
            .WithSummary("Registers a business, its first branch and its owner account.")
            .Produces<ApiResponse<ProvisionTenantResult>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    /// <summary>
    /// Anonymous by design: this is self-service sign-up, and there is no tenant to
    /// authenticate against yet. Rate limiting and bot protection sit in front of it.
    /// </summary>
    private static async Task<IResult> ProvisionAsync(
        ProvisionTenantRequest request,
        IDispatcher dispatcher,
        IErrorMessageLocalizer localizer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await dispatcher.SendAsync(
            new ProvisionTenantCommand(
                request.Name,
                request.Slug,
                request.CountryCode,
                request.CurrencyCode,
                request.DefaultLanguage,
                request.TimeZoneId,
                request.OwnerEmail,
                request.OwnerFullName,
                request.OwnerPassword,
                request.FirstBranchName),
            cancellationToken).ConfigureAwait(false);

        return result.ToHttpResult(httpContext, localizer, successStatusCode: StatusCodes.Status201Created);
    }
}
