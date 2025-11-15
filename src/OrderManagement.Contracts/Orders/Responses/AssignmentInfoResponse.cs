namespace OrderManagement.Contracts.Orders.Responses;

/// <summary>
/// Assignment Info Response DTO
/// </summary>
public sealed record AssignmentInfoResponse
{
    /// <summary>
    /// Gets the courier identifier.
    /// </summary>
    public Guid CourierId { get; init; }

    /// <summary>
    /// Gets the assigned at.
    /// </summary>
    public DateTimeOffset AssignedAt { get; init; }

    /// <summary>
    /// Gets the out for delivery at.
    /// </summary>
    public DateTimeOffset? OutForDeliveryAt { get; init; }

    /// <summary>
    /// Gets the delivered at.
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; init; }

    /// <summary>
    /// Gets the unable to deliver at.
    /// </summary>
    public DateTimeOffset? UnableToDeliverAt { get; init; }
}
