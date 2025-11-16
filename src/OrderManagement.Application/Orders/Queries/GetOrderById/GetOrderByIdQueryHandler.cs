using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Get Order ById Query Handler
/// </summary>
/// <seealso cref="IRequestHandler{GetOrderByIdQuery, OrderResult}" />
public sealed class GetOrderByIdQueryHandler(IOrderReadRepository readRepository, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetOrderByIdQuery, OrderResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="NotFoundException">Order</exception>
    public async Task<OrderResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to view orders.");

        var order = await readRepository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        ValidateCurrentUser(order);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }

    private void ValidateCurrentUser(Order order)
    {
        if (string.Equals(currentUser.Role, ApplicationRoles.Customer, StringComparison.OrdinalIgnoreCase) && order.CustomerId != currentUser.UserId)
            throw new ForbiddenException("Customers can only view their own orders.");

        if (string.Equals(currentUser.Role, ApplicationRoles.Delivery, StringComparison.OrdinalIgnoreCase) && order?.Assignment?.CourierId != currentUser.UserId)
            throw new ForbiddenException("You are not allowed to view this order.");
    }
}
