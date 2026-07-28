using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Identity.Application.Users.InviteUser;

/// <summary>Adds a user to the current tenant and assigns their starting roles and branches.</summary>
/// <param name="Email">Email the invitation is sent to. Unique within the tenant.</param>
/// <param name="FullName">Person's name.</param>
/// <param name="RoleIds">Roles granted on acceptance.</param>
/// <param name="BranchIds">Branches the user may work in.</param>
public sealed record InviteUserCommand(
    string Email,
    string FullName,
    IReadOnlyList<Guid> RoleIds,
    IReadOnlyList<Guid> BranchIds) : ICommand<Guid>;

/// <summary>Shape checks for <see cref="InviteUserCommand"/>.</summary>
public sealed class InviteUserValidator : IValidator<InviteUserCommand>
{
    public ValidationResult Validate(InviteUserCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return ValidationResult.Valid()
            .AddIf(string.IsNullOrWhiteSpace(request.Email), nameof(request.Email), ValidationCodes.Required)
            .AddIf(string.IsNullOrWhiteSpace(request.FullName), nameof(request.FullName), ValidationCodes.Required)
            .AddIf(request.FullName?.Length > 200, nameof(request.FullName), ValidationCodes.TooLong)
            .AddIf(request.RoleIds is null or { Count: 0 }, nameof(request.RoleIds), ValidationCodes.Required);
    }
}
