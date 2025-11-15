using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Order Filter
/// </summary>
public sealed record OrderFilter
{
    /// <summary>
    /// Gets the status.
    /// </summary>
    public OrderStatus? Status { get; init; }

    /// <summary>
    /// Gets the type.
    /// </summary>
    public OrderType? Type { get; init; }

    /// <summary>
    /// Gets the assigned courier identifier.
    /// </summary>
    public Guid? AssignedCourierId { get; init; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid? CustomerId { get; init; }
}
