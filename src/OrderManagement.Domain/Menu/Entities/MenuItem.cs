using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Domain.Menu.Entities;

/// <summary>
/// Menu Item
/// </summary>
public sealed class MenuItem
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the price.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is available.
    /// </summary>
    public bool IsAvailable { get; private set; } = true;

    /// <summary>
    /// Gets the category.
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    private MenuItem() { }

    private MenuItem(Guid id, string name, decimal price, string category, bool isAvailable)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.", "MenuItem.NameRequired");
        if (price < 0) throw new ValidationException("Price must be non-negative.", "MenuItem.PriceNegative");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim();
        Price = price;
        Category = category?.Trim() ?? string.Empty;
        IsAvailable = isAvailable;
    }

    /// <summary>
    /// Creates the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="price">The price.</param>
    /// <param name="category">The category.</param>
    /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
    public static MenuItem Create(string name, decimal price, string category, bool isAvailable = true)
        => new(Guid.NewGuid(), name, price, category, isAvailable);

    /// <summary>
    /// Sets the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <exception cref="ValidationException">Menu item name cannot be empty.</exception>
    public MenuItem SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Menu item name cannot be empty.");

        Name = name.Trim();
        return this;
    }

    /// <summary>
    /// Sets the price.
    /// </summary>
    /// <param name="price">The price.</param>
    /// <exception cref="ValidationException">Menu item price must be greater than zero.</exception>
    public MenuItem SetPrice(decimal price)
    {
        if (price <= 0) throw new ValidationException("Menu item price must be greater than zero.");

        Price = price;
        return this;
    }

    /// <summary>
    /// Sets the category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <exception cref="ValidationException">Menu item category cannot be empty.</exception>
    public MenuItem SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category)) throw new ValidationException("Menu item category cannot be empty.");

        Category = category.Trim();
        return this;
    }

    /// <summary>
    /// Sets the availability.
    /// </summary>
    /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
    public MenuItem SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        return this;
    }
}