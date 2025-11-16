using MediatR;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;

/// <summary>
/// Get Delivery Assignments Query
/// </summary>
/// <seealso cref="IRequest{GetDeliveryAssignmentsResult}" />
public sealed record GetDeliveryAssignmentsQuery() : IRequest<OrdersResult>;
