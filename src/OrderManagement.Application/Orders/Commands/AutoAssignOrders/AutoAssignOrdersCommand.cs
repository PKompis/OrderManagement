using MediatR;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Commands.AutoAssignOrders;

/// <summary>
/// Auto Assign Orders Command
/// </summary>
/// <seealso cref="IRequest{OrdersResult}" />
public sealed record AutoAssignOrdersCommand(int MaxOrders) : IRequest<OrdersResult>;