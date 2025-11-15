using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Application.Orders.Commands.AssignOrder;

/// <summary>
/// Assign Order Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{AssignOrderCommand, OrderResult}" />
public sealed class AssignOrderCommandHandler(IOrderRepository orderRepository, IStaffReadRepository staffReadRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<AssignOrderCommand, OrderResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Order
    /// or
    /// Staff
    /// </exception>
    /// <exception cref="BadRequestException">Cannot assign order to an inactive courier.</exception>
    public async Task<OrderResult> Handle(AssignOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        var courier = await staffReadRepository.GetByIdAsync(request.CourierId, cancellationToken) ?? throw new NotFoundException("Staff", request.CourierId);

        if (!courier.IsActive) throw new BadRequestException("Cannot assign order to an inactive courier.");


        if (courier.Role != StaffRole.Delivery) throw new BadRequestException("Staff member is not a delivery courier.");

        order.AssignCourier(request.CourierId, DateTimeOffset.UtcNow);

        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }
}
