using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Errors;
using Microsoft.AspNetCore.Http;

namespace ConfiOS.BuildingBlocks.Api.Middleware;

/// <summary>
/// Endpoint filter that rejects a request with no resolved tenant before the handler runs.
/// Applied to every business endpoint; platform endpoints such as sign-in opt out.
/// </summary>
public sealed class RequireTenantFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var tenantContext = context.HttpContext.RequestServices.GetService(typeof(ITenantContext)) as ITenantContext;

        if (tenantContext?.IsResolved != true)
        {
            var localizer = (IErrorMessageLocalizer)context.HttpContext.RequestServices
                .GetService(typeof(IErrorMessageLocalizer))!;

            return ResultExtensions.ToProblem(
                Error.Forbidden(ErrorCodes.TenantContextMissing),
                context.HttpContext.TraceIdentifier,
                localizer);
        }

        return await next(context).ConfigureAwait(false);
    }
}
