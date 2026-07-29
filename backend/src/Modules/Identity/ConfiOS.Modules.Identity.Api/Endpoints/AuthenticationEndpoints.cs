using System.Security.Claims;
using ConfiOS.BuildingBlocks.Api.Contracts;
using ConfiOS.BuildingBlocks.Api.Localization;
using ConfiOS.BuildingBlocks.Api.Results;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Identity.Api.Contracts;
using ConfiOS.Modules.Identity.Application.Authentication.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ConfiOS.Modules.Identity.Api.Endpoints;

/// <summary>Sign-in and session endpoints.</summary>
public static class AuthenticationEndpoints
{
    /// <summary>Maps the authentication routes under <c>/api/v1/auth</c>.</summary>
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var group = endpoints
            .MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Signs a user in and returns an access token.")
            .Produces<ApiResponse<LoginResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", CurrentUser)
            .RequireAuthorization()
            .WithName("CurrentUser")
            .WithSummary("Returns the identity carried by the current access token.")
            .Produces<ApiResponse<CurrentUserResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IDispatcher dispatcher,
        IErrorMessageLocalizer localizer,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await dispatcher.SendAsync(
            new LoginCommand(request.BusinessHandle, request.Email, request.Password),
            cancellationToken).ConfigureAwait(false);

        return result.ToHttpResult(httpContext, localizer, successStatusCode: StatusCodes.Status200OK);
    }

    /// <summary>
    /// Reads the identity straight from the validated token, so it doubles as a cheap way for
    /// a client to confirm its session is still valid on start-up.
    /// </summary>
    private static IResult CurrentUser(HttpContext httpContext)
    {
        var principal = httpContext.User;
        var response = new CurrentUserResponse(
            principal.FindFirstValue("sub"),
            principal.FindFirstValue("tenant_id"),
            principal.FindFirstValue("email"),
            principal.FindAll("permission").Select(claim => claim.Value).ToList());

        return Microsoft.AspNetCore.Http.Results.Json(
            new ApiResponse<CurrentUserResponse>(response, null, httpContext.TraceIdentifier));
    }
}
