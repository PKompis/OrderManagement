namespace OrderManagement.Contracts.Orders.Enums;

/// <summary>
/// Order Status Dto
/// </summary>
public enum OrderStatusDto
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