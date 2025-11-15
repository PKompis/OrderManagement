namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Place Order Item Model
/// </summary>
public sealed record PlaceOrderItemModel
{
    /// <summary>
    /// Gets the menu item identifier.
    /// </summary>
    public Guid MenuItemId { get; init; }

    /// <summary>
    /// Gets the quantity.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the notes.
    /// </summary>
    public string? Notes { get; init; }
}
