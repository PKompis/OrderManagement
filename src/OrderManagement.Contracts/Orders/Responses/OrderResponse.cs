using OrderManagement.Contracts.Orders.Enums;

namespace OrderManagement.Contracts.Orders.Responses;

/// <summary>
/// Order Response DTO
/// </summary>
public sealed record OrderResponse
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
    public OrderTypeDto Type { get; init; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public OrderStatusDto Status { get; init; }

    /// <summary>
    /// Gets the created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the delivery time needed.
    /// </summary>
    public TimeSpan? DeliveryTimeNeeded { get; init; }

    /// <summary>
    /// Gets the delivery address.
    /// </summary>
    public DeliveryAddressResponse? DeliveryAddress { get; init; }

    /// <summary>
    /// Gets the assignment.
    /// </summary>
    public AssignmentInfoResponse? Assignment { get; init; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<OrderItemResponse> Items { get; init; } = [];

    /// <summary>
    /// Gets the total.
    /// </summary>
    public decimal Total { get; init; }

    /// <summary>
    /// Gets the estimated delivery time.
    /// </summary>
    public DateTimeOffset? EstimatedDeliveryTime { get; init; }
}