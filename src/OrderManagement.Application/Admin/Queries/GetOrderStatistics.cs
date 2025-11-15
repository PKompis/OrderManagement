using MediatR;

namespace OrderManagement.Application.Admin.Queries;

/// <summary>
/// Get Order Statistics Query
/// </summary>
/// <seealso cref="IRequest{OrderStatisticsResult}" />
public sealed record GetOrderStatisticsQuery : IRequest<OrderStatisticsResult>;
