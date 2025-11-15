using OrderManagement.Application.Orders.Models;

namespace OrderManagement.Application.Orders.Results;

/// <summary>
/// Order Result
/// </summary>
public sealed record OrderResult
{
    /// <summary>
    /// Gets the order.
    /// </summary>
    public OrderModel Order { get; init; } = default!;
}
