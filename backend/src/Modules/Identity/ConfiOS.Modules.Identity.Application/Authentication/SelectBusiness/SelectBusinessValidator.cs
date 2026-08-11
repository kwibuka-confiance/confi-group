using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Identity.Application.Authentication.SelectBusiness;

/// <summary>Shape checks for <see cref="SelectBusinessCommand"/>.</summary>
public sealed class SelectBusinessValidator : IValidator<SelectBusinessCommand>
{
    public ValidationResult Validate(SelectBusinessCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidationResult.Valid();
        result.AddIf(
            string.IsNullOrWhiteSpace(request.SelectionToken),
            nameof(request.SelectionToken),
            ValidationCodes.Required);
        result.AddIf(
            request.TenantId == Guid.Empty,
            nameof(request.TenantId),
            ValidationCodes.Required);

        return result;
    }
}
