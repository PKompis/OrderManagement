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
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Update Order Status Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{UpdateOrderStatusCommand, OrderResult}" />
public sealed class UpdateOrderStatusCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<UpdateOrderStatusCommand, OrderResult>
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
    /// <exception cref="BadRequestException"></exception>
    public async Task<OrderResult> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to update order status.");

        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        if (string.Equals(currentUser.Role, ApplicationRoles.Kitchen, StringComparison.OrdinalIgnoreCase)) EnforceKitchenRules(order, request.NewStatus);
        else if (string.Equals(currentUser.Role, ApplicationRoles.Delivery, StringComparison.OrdinalIgnoreCase)) EnforceDeliveryRules(order, request.NewStatus, currentUser.UserId.Value);
        else if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase)) throw new ForbiddenException("You are not allowed to update order status.");

        order.ChangeStatus(request.NewStatus, DateTimeOffset.UtcNow);

        await repository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }

    private static void EnforceKitchenRules(Order order, OrderStatus targetStatus)
    {
        // Kitchen cannot set delivery-completion statuses
        if (targetStatus is OrderStatus.Delivered or OrderStatus.UnableToDeliver or OrderStatus.OutForDelivery) 
            throw new ForbiddenException("Kitchen staff cannot set delivery completion statuses.");

        // Kitchen can cancel only before preparation has started (Pending)
        if (targetStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Pending) 
            throw new BadRequestException("Order can only be cancelled before preparation starts.");
    }

    private static void EnforceDeliveryRules(Order order, OrderStatus targetStatus, Guid courierId)
    {
        if (order.Type != OrderType.Delivery) throw new ForbiddenException("Delivery staff can only update delivery orders.");
        if (order?.Assignment?.CourierId != courierId) throw new ForbiddenException("You can only update the status of orders assigned to you.");
        if (targetStatus is not (OrderStatus.Delivered or OrderStatus.UnableToDeliver)) throw new ForbiddenException("Delivery staff can only mark orders as Delivered or UnableToDeliver.");
    }
}
