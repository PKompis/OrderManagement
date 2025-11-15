using OrderManagement.Application.Orders.Models;

namespace OrderManagement.Application.Orders.Results;

/// <summary>
/// Orders Result
/// </summary>
public sealed record OrdersResult
{
    /// <summary>
    /// Gets the orders.
    /// </summary>
    public IReadOnlyCollection<OrderModel> Orders { get; init; } = [];
}
