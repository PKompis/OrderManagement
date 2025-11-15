using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Domain.Orders.ValueObjects;

/// <summary>
/// Order Item
/// </summary>
public sealed record OrderItem
{
    /// <summary>
    /// Gets the menu item identifier.
    /// </summary>
    public Guid MenuItemId { get; init; }

    /// <summary>
    /// Gets the name. Repetition for history purposes.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the unit price. Repetition for history purposes.
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
    public decimal Total => UnitPrice * Quantity;

    private OrderItem(Guid menuItemId, string name, decimal unitPrice, int quantity, string? notes)
    {
        if (menuItemId == Guid.Empty) throw new ValidationException("MenuItemId is required.", "OrderItem.MenuItemIdRequired");
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Item name is required.", "OrderItem.NameRequired");
        if (unitPrice < 0) throw new ValidationException("Unit price must be non-negative.", "OrderItem.UnitPriceNegative");
        if (quantity <= 0) throw new ValidationException("Quantity must be greater than zero.", "OrderItem.QuantityNotPositive");

        MenuItemId = menuItemId;
        Name = name.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Creates the specified menu item identifier.
    /// </summary>
    /// <param name="menuItemId">The menu item identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="quantity">The quantity.</param>
    /// <param name="notes">The notes.</param>
    public static OrderItem Create(Guid menuItemId, string name, decimal unitPrice, int quantity, string? notes = null)
        => new(menuItemId, name, unitPrice, quantity, notes);
}