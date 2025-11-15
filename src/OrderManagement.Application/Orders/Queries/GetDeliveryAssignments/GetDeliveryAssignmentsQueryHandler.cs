using AutoMapper;
using MediatR;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;

/// <summary>
/// Get Delivery Assignments Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetDeliveryAssignmentsQuery, OrdersResult}" />
public sealed class GetDeliveryAssignmentsQueryHandler(IOrderReadRepository readRepository, IMapper mapper) : IRequestHandler<GetDeliveryAssignmentsQuery, OrdersResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    public async Task<OrdersResult> Handle(GetDeliveryAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var filter = new OrderFilter { AssignedCourierId = request.CourierId };

        var orders = await readRepository.GetByFilterAsync(filter, cancellationToken: cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<OrderModel>>(orders);

        return new OrdersResult { Orders = models };
    }
}
