using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Identity.Application.Authentication.Login;

namespace ConfiOS.Modules.Identity.Application.Authentication.SelectBusiness;

/// <summary>
/// Completes a multi-business sign-in by choosing which business to continue into.
/// </summary>
/// <param name="SelectionToken">Token issued when the credentials unlocked several businesses.</param>
/// <param name="TenantId">The chosen business.</param>
public sealed record SelectBusinessCommand(string SelectionToken, Guid TenantId)
    : ICommand<AuthenticatedSession>;
