using AutoMapper;
using MediatR;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

/// <summary>
/// Get Orders Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrdersQuery, OrdersResult}" />
public sealed class GetOrdersQueryHandler(IOrderReadRepository readRepository, IMapper mapper) : IRequestHandler<GetOrdersQuery, OrdersResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    public async Task<OrdersResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var filter = new OrderFilter
        {
            Status = request.Status,
            Type = request.Type,
            AssignedCourierId = request.AssignedCourierId,
            CustomerId = request.CustomerId
        };

        var orders = await readRepository.GetByFilterAsync(filter, cancellationToken: cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<OrderModel>>(orders);

        return new OrdersResult { Orders = models };
    }
}