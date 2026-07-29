using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

/// <summary>Shape and range checks for <see cref="CreateProductCommand"/>.</summary>
public sealed class CreateProductValidator : IValidator<CreateProductCommand>
{
    public ValidationResult Validate(CreateProductCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidationResult.Valid();

        result.AddIf(string.IsNullOrWhiteSpace(request.Name), nameof(request.Name), ValidationCodes.Required);
        result.AddIf(request.Name?.Length > 200, nameof(request.Name), ValidationCodes.TooLong);
        result.AddIf(string.IsNullOrWhiteSpace(request.Sku), nameof(request.Sku), ValidationCodes.Required);
        result.AddIf(request.Sku?.Length > 64, nameof(request.Sku), ValidationCodes.TooLong);
        result.AddIf(request.CurrencyCode?.Length != 3, nameof(request.CurrencyCode), ValidationCodes.InvalidFormat);
        result.AddIf(request.PriceAmount < 0, nameof(request.PriceAmount), ValidationCodes.MustBePositive);

        return result;
    }
}
