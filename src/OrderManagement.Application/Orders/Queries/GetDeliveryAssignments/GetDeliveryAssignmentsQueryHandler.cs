using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;

/// <summary>
/// Get Delivery Assignments Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetDeliveryAssignmentsQuery, OrdersResult}" />
public sealed class GetDeliveryAssignmentsQueryHandler(IOrderReadRepository readRepository, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetDeliveryAssignmentsQuery, OrdersResult>
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
        ValidateCurrentUser();

        var filter = new OrderFilter
        {
            AssignedCourierId = currentUser.UserId!.Value,
            Status = OrderStatus.OutForDelivery,
            Type = OrderType.Delivery
        };

        var orders = await readRepository.GetByFilterAsync(filter, cancellationToken: cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<OrderModel>>(orders);

        return new OrdersResult { Orders = models };
    }

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to view delivery assignments.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Delivery, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Only delivery staff can see delivery assignments.");
    }
}
