using MediatR;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Update Order Status Command
/// </summary>
/// <seealso cref="IRequest{UpdateOrderStatusResult}" />
public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<OrderResult>;