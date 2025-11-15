namespace OrderManagement.Contracts.Orders.Requests;

/// <summary>
/// Place Order Item Request
/// </summary>
public sealed record PlaceOrderItemRequest
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
