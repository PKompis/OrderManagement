namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Order Item Model
/// </summary>
public sealed record OrderItemModel
{
    /// <summary>
    /// Gets the menu item identifier.
    /// </summary>
    public Guid MenuItemId { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unit price.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Gets the quantity.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the notes.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Gets the total.
    /// </summary>
    public decimal Total { get; init; }
}
