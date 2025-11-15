namespace OrderManagement.Domain.Orders.Enums;

/// <summary>
/// Order Status
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Preparing = 1,

    // Pickup-only
    ReadyForPickup = 2,

    // Delivery-only
    ReadyForDelivery = 3,
    OutForDelivery = 4,
    Delivered = 5,

    UnableToDeliver = 6,
    Cancelled = 7
}