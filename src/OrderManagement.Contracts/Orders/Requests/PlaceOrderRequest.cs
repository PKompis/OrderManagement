using OrderManagement.Contracts.Orders.Enums;

namespace OrderManagement.Contracts.Orders.Requests;

/// <summary>
/// Place Order Request DTO
/// </summary>
public sealed record PlaceOrderRequest
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    public OrderTypeDto Type { get; init; }

    /// <summary>
    /// Gets the delivery address.
    /// </summary>
    public DeliveryAddressRequest? DeliveryAddress { get; init; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<PlaceOrderItemRequest> Items { get; init; } = [];
}
