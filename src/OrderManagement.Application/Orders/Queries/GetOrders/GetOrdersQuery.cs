using MediatR;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

/// <summary>
/// Get Orders Query
/// </summary>
/// <seealso cref="IRequest{OrdersResult}" />
public sealed record GetOrdersQuery : IRequest<OrdersResult>
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
