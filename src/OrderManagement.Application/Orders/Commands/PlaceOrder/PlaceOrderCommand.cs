using MediatR;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Commands.PlaceOrder;

/// <summary>
/// Place Order Command
/// </summary>
/// <seealso cref="IRequest{PlaceOrderResult}" />
public sealed record PlaceOrderCommand : IRequest<OrderResult>
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Gets the delivery address.
    /// </summary>
    public DeliveryAddressModel? DeliveryAddress { get; init; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IReadOnlyCollection<PlaceOrderItemModel> Items { get; init; } = [];
}
