namespace OrderManagement.Contracts.Menu.Requests;

/// <summary>
/// Upsert Menu Item Request
/// </summary>
public sealed record UpsertMenuItemRequest
{
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
    public bool IsAvailable { get; init; } = true;
}
