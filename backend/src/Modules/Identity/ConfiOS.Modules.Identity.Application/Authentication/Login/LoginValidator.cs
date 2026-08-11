using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Identity.Application.Authentication.Login;

/// <summary>Shape checks for <see cref="LoginCommand"/>. Credentials are verified in the handler.</summary>
public sealed class LoginValidator : IValidator<LoginCommand>
{
    public ValidationResult Validate(LoginCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidationResult.Valid();
        result.AddIf(
            string.IsNullOrWhiteSpace(request.Email),
            nameof(request.Email),
            ValidationCodes.Required);
        result.AddIf(
            string.IsNullOrWhiteSpace(request.Password),
            nameof(request.Password),
            ValidationCodes.Required);

        return result;
    }
}
