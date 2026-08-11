using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Products.SetProductStatus;

/// <summary>
/// Withdraws a product from sale or restores it, recording who did so.
/// </summary>
/// <remarks>
/// Withdrawing a product stops it being sold, which is why it is audited: someone
/// has to be able to answer why a line the depot stocks stopped appearing.
/// </remarks>
/// <param name="products">Product repository (already tenant-scoped).</param>
/// <param name="tenantContext">The resolved tenant, branch and user.</param>
/// <param name="auditLogger">Records the change alongside it, in one transaction.</param>
/// <param name="unitOfWork">Commits the change.</param>
public sealed class SetProductStatusHandler(
    IProductRepository products,
    ITenantContext tenantContext,
    ICatalogAuditLogger auditLogger,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<SetProductStatusCommand>
{
    public async Task<Result> HandleAsync(
        SetProductStatusCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var product = await products.GetAsync(command.ProductId, cancellationToken).ConfigureAwait(false);
        if (product is null)
        {
            return Result.Failure(Error.NotFound(CatalogErrorCodes.ProductNotFound));
        }

        // Already in the requested state: succeed without writing an audit entry that
        // would suggest something changed.
        if (product.IsActive == command.IsActive)
        {
            return Result.Success();
        }

        if (command.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Archive();
        }

        await auditLogger.RecordAsync(
            new AuditEntry(
                command.IsActive ? "catalog.product-restored" : "catalog.product-archived",
                nameof(Product),
                product.Id,
                tenantContext.TenantId,
                tenantContext.BranchId,
                tenantContext.UserId,
                new Dictionary<string, object?>
                {
                    ["sku"] = product.Sku,
                    ["name"] = product.Name,
                }),
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
