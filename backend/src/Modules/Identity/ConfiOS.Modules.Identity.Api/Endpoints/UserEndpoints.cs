using ConfiOS.BuildingBlocks.Api.Contracts;
using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Middleware;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Identity.Api.Contracts;
using ConfiOS.Modules.Identity.Application.Users.InviteUser;
using ConfiOS.Modules.Identity.Domain.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ConfiOS.Modules.Identity.Api.Endpoints;

/// <summary>User management endpoints for the current tenant.</summary>
public static class UserEndpoints
{
    /// <summary>Maps the user routes under <c>/api/v1/users</c>.</summary>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var group = endpoints
            .MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization()
            .AddEndpointFilter<RequireTenantFilter>();

        group.MapPost("/", InviteAsync)
            .RequireAuthorization(Permissions.Users.Invite)
            .WithName("InviteUser")
            .WithSummary("Invites a person to join the current business.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> InviteAsync(
        InviteUserRequest request,
        IDispatcher dispatcher,
        IErrorMessageLocalizer localizer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await dispatcher.SendAsync(
            new InviteUserCommand(
                request.Email,
                request.FullName,
                request.RoleIds ?? [],
                request.BranchIds ?? []),
            cancellationToken).ConfigureAwait(false);

        return result.ToHttpResult(httpContext, localizer, successStatusCode: StatusCodes.Status201Created);
    }
}
