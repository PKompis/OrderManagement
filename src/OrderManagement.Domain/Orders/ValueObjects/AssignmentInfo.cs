using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Domain.Orders.ValueObjects;

/// <summary>
/// Courier assignment and delivery milestones for a delivery order.
/// </summary>
public sealed record AssignmentInfo
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

    private AssignmentInfo() { }

    private AssignmentInfo(
        Guid courierId,
        DateTimeOffset assignedAt,
        DateTimeOffset? outForDeliveryAt,
        DateTimeOffset? deliveredAt,
        DateTimeOffset? unableToDeliverAt
    )
    {
        if (courierId == Guid.Empty) throw new ValidationException("CourierId is required.", "Assignment.CourierIdRequired");

        if (outForDeliveryAt is not null && outForDeliveryAt < assignedAt) throw new ValidationException("OutForDeliveryAt cannot be before AssignedAt.", "Assignment.OutBeforeAssigned");

        if (deliveredAt is not null)
        {
            if (outForDeliveryAt is null) throw new ValidationException("Cannot set DeliveredAt before OutForDeliveryAt.", "Assignment.DeliveredBeforeOut");
            if (deliveredAt < outForDeliveryAt) throw new ValidationException("DeliveredAt cannot be before OutForDeliveryAt.", "Assignment.DeliveredBeforeOutTime");
        }

        if (unableToDeliverAt is not null)
        {
            if (outForDeliveryAt is null) throw new ValidationException("Cannot set UnableToDeliverAt before OutForDeliveryAt.", "Assignment.UnableBeforeOut");
            if (unableToDeliverAt < outForDeliveryAt) throw new ValidationException("UnableToDeliverAt cannot be before OutForDeliveryAt.", "Assignment.UnableBeforeOutTime");
        }

        CourierId = courierId;
        AssignedAt = assignedAt;
        OutForDeliveryAt = outForDeliveryAt;
        DeliveredAt = deliveredAt;
        UnableToDeliverAt = unableToDeliverAt;
    }

    public static AssignmentInfo Create(Guid courierId, DateTimeOffset now) => new(courierId, now, null, null, null);

    /// <summary>
    /// Marks the out for delivery.
    /// </summary>
    /// <param name="now">The now.</param>
    /// <exception cref="ValidationException">OutForDeliveryAt cannot be before AssignedAt., Assignment.OutBeforeAssigned</exception>
    public AssignmentInfo MarkOutForDelivery(DateTimeOffset now)
    {
        if (OutForDeliveryAt is not null) return this;
        if (now < AssignedAt) throw new ValidationException("OutForDeliveryAt cannot be before AssignedAt.", "Assignment.OutBeforeAssigned");

        return this with { OutForDeliveryAt = now };
    }

    /// <summary>
    /// Marks the delivered.
    /// </summary>
    /// <param name="now">The now.</param>
    /// <exception cref="ValidationException">
    /// Cannot mark Delivered before OutForDelivery., Assignment.DeliveredBeforeOut
    /// or
    /// DeliveredAt cannot be before OutForDeliveryAt., Assignment.DeliveredBeforeOutTime
    /// </exception>
    public AssignmentInfo MarkDelivered(DateTimeOffset now)
    {
        if (DeliveredAt is not null) return this;
        if (OutForDeliveryAt is null) throw new ValidationException("Cannot mark Delivered before OutForDelivery.", "Assignment.DeliveredBeforeOut");
        if (now < OutForDeliveryAt) throw new ValidationException("DeliveredAt cannot be before OutForDeliveryAt.", "Assignment.DeliveredBeforeOutTime");
        
        return this with { DeliveredAt = now };
    }

    /// <summary>
    /// Marks the unable to deliver.
    /// </summary>
    /// <param name="now">The now.</param>
    /// <exception cref="ValidationException">
    /// Cannot mark UnableToDeliver before OutForDelivery., Assignment.UnableBeforeOut
    /// or
    /// UnableToDeliverAt cannot be before OutForDeliveryAt., Assignment.UnableBeforeOutTime
    /// </exception>
    public AssignmentInfo MarkUnableToDeliver(DateTimeOffset now)
    {
        if (UnableToDeliverAt is not null) return this;
        if (OutForDeliveryAt is null) throw new ValidationException("Cannot mark UnableToDeliver before OutForDelivery.", "Assignment.UnableBeforeOut");
        if (now < OutForDeliveryAt) throw new ValidationException("UnableToDeliverAt cannot be before OutForDeliveryAt.", "Assignment.UnableBeforeOutTime");
        
        return this with { UnableToDeliverAt = now };
    }
}
