using OrderManagement.Application.Menu.Models;

namespace OrderManagement.Application.Menu.Results;

/// <summary>
/// Menu Item Result
/// </summary>
public sealed record MenuItemResult
{
    /// <summary>
    /// Gets the item.
    /// </summary>
    public MenuItemModel Item { get; init; } = default!;
}
