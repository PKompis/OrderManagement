using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

/// <summary>
/// Get Orders Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrdersQuery, OrdersResult}" />
public sealed class GetOrdersQueryHandler(IOrderReadRepository readRepository, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetOrdersQuery, OrdersResult>
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
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to view orders.");

        var filter = RetrieveFilter(request);

        var orders = await readRepository.GetByFilterAsync(filter, cancellationToken: cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<OrderModel>>(orders);

        return new OrdersResult { Orders = models };
    }

    private OrderFilter RetrieveFilter(GetOrdersQuery request)
    {
        var userId = currentUser.UserId!.Value;

        if (string.Equals(currentUser.Role, ApplicationRoles.Customer, StringComparison.OrdinalIgnoreCase)) return new OrderFilter { Status = request.Status, Type = request.Type, CustomerId = userId };

        if (string.Equals(currentUser.Role, ApplicationRoles.Delivery, StringComparison.OrdinalIgnoreCase)) return new OrderFilter { Status = request.Status, Type = request.Type, AssignedCourierId = userId };

        if (
            string.Equals(currentUser.Role, ApplicationRoles.Kitchen, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase)
        )
        {
            return new OrderFilter
            {
                Status = request.Status,
                Type = request.Type,
                AssignedCourierId = request.AssignedCourierId,
                CustomerId = request.CustomerId
            };
        }

        throw new ForbiddenException("You are not allowed to view orders.");
    }
}