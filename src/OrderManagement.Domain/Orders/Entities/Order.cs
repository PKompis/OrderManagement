using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.Exceptions;
using OrderManagement.Domain.Orders.Rules;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.Domain.Orders.Entities;

/// <summary>
/// Order entity
/// </summary>
public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the type.
    /// </summary>
    public OrderType Type { get; private set; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    /// <summary>
    /// Gets the created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the delivery time needed.
    /// </summary>
    public TimeSpan? DeliveryTimeNeeded { get; private set; }

    /// <summary>
    /// Gets the delivery address. (only for Delivery)
    /// </summary>
    public DeliveryAddress? DeliveryAddress { get; private set; }

    /// <summary>
    /// Gets the assignment. (Not filled until a courier is assigned)
    /// </summary>
    public AssignmentInfo? Assignment { get; private set; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items;

    public decimal Total => _items.Sum(i => i.Total);

    private Order() { }

    private Order(Guid id, Guid customerId, OrderType type, IEnumerable<OrderItem> items, DeliveryAddress? deliveryAddress)
    {
        if (customerId == Guid.Empty) throw new ValidationException("CustomerId is required.", "Order.CustomerIdRequired");

        var list = (items ?? throw new ValidationException("Items collection is required.", "Order.ItemsRequired")).ToList();
        if (list.Count == 0) throw new ValidationException("Order must contain at least one item.", "Order.ItemsEmpty");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        CustomerId = customerId;
        Type = type;
        _items.AddRange(list);

        if (type == OrderType.Delivery)
        {
            if (deliveryAddress is null) throw new ValidationException("Delivery address is required for delivery orders.", "Order.DeliveryAddressRequired");

            DeliveryAddress = deliveryAddress;
        }
        else if (deliveryAddress is not null)
        {
            throw new ValidationException("Pickup orders must not include a delivery address.", "Order.DeliveryAddressNotAllowed");
        }
    }

    /// <summary>
    /// Generic factory for both pickup and delivery orders.
    /// </summary>
    public static Order Create(
        Guid customerId,
        OrderType type,
        IEnumerable<OrderItem> items,
        DeliveryAddress? deliveryAddress = null
    ) => new(Guid.NewGuid(), customerId, type, items, deliveryAddress);

    /// <summary>
    /// Changes the order status if the transition is allowed for the order type.
    /// Also keeps Assignment milestones in sync for delivery orders.
    /// </summary>
    public void ChangeStatus(OrderStatus target, DateTimeOffset now)
    {
        if (Status == target) return;

        if (!OrderStatusRules.CanTransition(Type, Status, target)) throw new InvalidOrderTransitionException(Type, Status, target);

        Status = target;

        // Mirror lifecycle timestamps on Assignment for delivery orders
        if (Type == OrderType.Delivery && Assignment is not null)
        {
            if (target == OrderStatus.OutForDelivery) Assignment = Assignment.MarkOutForDelivery(now);
            else if (target == OrderStatus.Delivered) Assignment = Assignment.MarkDelivered(now);
            else if (target == OrderStatus.UnableToDeliver) Assignment = Assignment.MarkUnableToDeliver(now);
        }

        // Guard: for delivery milestones we require an assignment first
        if (target is OrderStatus.OutForDelivery or OrderStatus.Delivered or OrderStatus.UnableToDeliver)
        {
            if (Type != OrderType.Delivery) throw new ValidationException("Only delivery orders can progress to delivery states.", "Order.DeliveryStatesOnPickup");
            if (Assignment is null) throw new ValidationException("Cannot progress to delivery states without an assigned courier.", "Order.MissingAssignment");
        }
    }

    /// <summary>
    /// Assigns a courier to a delivery order.
    /// </summary>
    public void AssignCourier(Guid courierId, DateTimeOffset now)
    {
        if (Type != OrderType.Delivery) throw new ValidationException("Cannot assign a courier to a pickup order.", "Order.AssignCourierOnPickup");
        if (courierId == Guid.Empty) throw new ValidationException("CourierId is required.", "Order.CourierIdRequired");

        if (Status is not (OrderStatus.Pending or OrderStatus.Preparing or OrderStatus.ReadyForDelivery)) throw new ValidationException("Order is not in an assignable state.", "Order.Status");

        Assignment = AssignmentInfo.Create(courierId, now);
    }

    /// <summary>
    /// Sets the delivery time needed.
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <returns></returns>
    /// <exception cref="ValidationException">
    /// Delivery time is only valid for delivery orders., Order.DeliveryTimeOnPickup
    /// or
    /// Delivery duration must be positive., Order.DeliveryTimeInvalid
    /// </exception>
    public void SetDeliveryTimeNeeded(TimeSpan duration)
    {
        if (Type != OrderType.Delivery) throw new ValidationException("Delivery time is only valid for delivery orders.", "Order.DeliveryTimeOnPickup");

        if (duration <= TimeSpan.Zero) throw new ValidationException("Delivery duration must be positive.", "Order.DeliveryTimeInvalid");

        DeliveryTimeNeeded = duration;
    }
}