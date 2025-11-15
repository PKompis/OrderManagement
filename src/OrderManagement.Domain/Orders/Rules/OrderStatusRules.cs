using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Domain.Orders.Rules;

/// <summary>
/// Defines which order-status transitions are allowed for each order type.
/// </summary>
public static class OrderStatusRules
{
    //Pickup Flow
    private static readonly HashSet<(OrderStatus From, OrderStatus To)> _pickupTransitions =
    [
        (OrderStatus.Pending,   OrderStatus.Preparing),
        (OrderStatus.Pending,   OrderStatus.Cancelled),
        (OrderStatus.Preparing, OrderStatus.ReadyForPickup)
    ];

    // Delivery flow
    private static readonly HashSet<(OrderStatus From, OrderStatus To)> _deliveryTransitions =
    [
        (OrderStatus.Pending,          OrderStatus.Preparing),
        (OrderStatus.Pending,          OrderStatus.Cancelled),
        (OrderStatus.Preparing,        OrderStatus.ReadyForDelivery),
        (OrderStatus.ReadyForDelivery, OrderStatus.OutForDelivery),
        (OrderStatus.OutForDelivery,   OrderStatus.Delivered),
        (OrderStatus.OutForDelivery,   OrderStatus.UnableToDeliver)
    ];

    /// <summary>
    /// Returns true if the transition is valid for the given order type.
    /// </summary>
    public static bool CanTransition(OrderType type, OrderStatus from, OrderStatus to) =>
        type switch
        {
            OrderType.Pickup => _pickupTransitions.Contains((from, to)),
            OrderType.Delivery => _deliveryTransitions.Contains((from, to)),
            _ => false
        };
}
