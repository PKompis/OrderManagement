using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Order Model
/// </summary>
public sealed record OrderModel
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Gets the type.
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Gets the created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }


    /// <summary>
    /// Gets the delivery address.
    /// </summary>
    public DeliveryAddressModel? DeliveryAddress { get; init; }

    /// <summary>
    /// Gets the assignment.
    /// </summary>
    public AssignmentInfoModel? Assignment { get; init; }

    /// <summary>
    /// Gets the delivery time needed.
    /// </summary>
    public TimeSpan? DeliveryTimeNeeded { get; init; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<OrderItemModel> Items { get; init; } = [];

    /// <summary>
    /// Gets the total.
    /// </summary>
    public decimal Total { get; init; }

    /// <summary>
    /// Gets the estimated delivery time.
    /// </summary>
    public DateTimeOffset? EstimatedDeliveryTime => DeliveryTimeNeeded is null ? null : CreatedAt + DeliveryTimeNeeded;
}
