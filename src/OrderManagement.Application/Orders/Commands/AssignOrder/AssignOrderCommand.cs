using MediatR;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Commands.AssignOrder;

/// <summary>
/// Assign Order Command
/// </summary>
/// <seealso cref="IRequest{AssignOrderResult}" />
public sealed record AssignOrderCommand(Guid OrderId, Guid CourierId) : IRequest<OrderResult>;