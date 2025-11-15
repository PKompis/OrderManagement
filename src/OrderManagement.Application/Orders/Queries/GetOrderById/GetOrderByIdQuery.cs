using MediatR;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Get Order By Id Query
/// </summary>
/// <seealso cref="IRequest{OrderResult}" />
public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderResult>;
