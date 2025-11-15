namespace OrderManagement.Contracts.Menu.Responses;

/// <summary>
/// Menu Item Response DTO
/// </summary>
public sealed record MenuItemResponse
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the price.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Gets the category.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this instance is available.
    /// </summary>
    public bool IsAvailable { get; init; }
}
