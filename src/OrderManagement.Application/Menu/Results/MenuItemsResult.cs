using OrderManagement.Application.Menu.Models;

namespace OrderManagement.Application.Menu.Results;

/// <summary>
/// Menu Items Result
/// </summary>
public sealed record MenuItemsResult
{
    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<MenuItemModel> Items { get; init; } = [];
}
