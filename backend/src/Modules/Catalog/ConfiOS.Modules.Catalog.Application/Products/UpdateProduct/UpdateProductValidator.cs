using ConfiOS.BuildingBlocks.Application.Validation;

namespace ConfiOS.Modules.Catalog.Application.Products.UpdateProduct;

/// <summary>Shape and range checks for <see cref="UpdateProductCommand"/>.</summary>
public sealed class UpdateProductValidator : IValidator<UpdateProductCommand>
{
    public ValidationResult Validate(UpdateProductCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ValidationResult.Valid();

        result.AddIf(request.ProductId == Guid.Empty, nameof(request.ProductId), ValidationCodes.Required);
        result.AddIf(string.IsNullOrWhiteSpace(request.Name), nameof(request.Name), ValidationCodes.Required);
        result.AddIf(request.Name?.Length > 200, nameof(request.Name), ValidationCodes.TooLong);
        result.AddIf(string.IsNullOrWhiteSpace(request.Sku), nameof(request.Sku), ValidationCodes.Required);
        result.AddIf(request.Sku?.Length > 64, nameof(request.Sku), ValidationCodes.TooLong);
        result.AddIf(request.CurrencyCode?.Length != 3, nameof(request.CurrencyCode), ValidationCodes.InvalidFormat);
        result.AddIf(request.PriceAmount < 0, nameof(request.PriceAmount), ValidationCodes.MustBePositive);
        result.AddIf(request.CostAmount < 0, nameof(request.CostAmount), ValidationCodes.MustBePositive);
        result.AddIf(request.DepositAmount < 0, nameof(request.DepositAmount), ValidationCodes.MustBePositive);
        result.AddIf(request.Description?.Length > 2000, nameof(request.Description), ValidationCodes.TooLong);

        foreach (var packaging in request.Packagings ?? [])
        {
            result.AddIf(
                string.IsNullOrWhiteSpace(packaging.UnitCode),
                nameof(request.Packagings),
                ValidationCodes.Required);
            result.AddIf(
                packaging.QuantityInBaseUnit <= 0,
                nameof(request.Packagings),
                ValidationCodes.MustBePositive);
            result.AddIf(
                packaging.SellingPriceAmount < 0,
                nameof(request.Packagings),
                ValidationCodes.MustBePositive);
        }

        return result;
    }
}
