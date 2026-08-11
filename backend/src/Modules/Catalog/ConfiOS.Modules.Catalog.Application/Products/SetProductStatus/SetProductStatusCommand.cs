using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Catalog.Application.Products.SetProductStatus;

/// <summary>
/// Takes a product out of sale, or puts it back.
/// </summary>
/// <remarks>
/// Archiving never deletes. A product appears on past orders and stock movements,
/// so removing the row would falsify history; it is only withdrawn from what can be
/// sold next.
/// </remarks>
/// <param name="ProductId">Which product to change.</param>
/// <param name="IsActive">True to sell it again, false to withdraw it.</param>
public sealed record SetProductStatusCommand(Guid ProductId, bool IsActive) : ICommand;
