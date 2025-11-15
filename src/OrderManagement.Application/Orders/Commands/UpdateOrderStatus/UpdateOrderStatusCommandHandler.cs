using AutoMapper;
using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;

namespace OrderManagement.Application.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Update Order Status Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{UpdateOrderStatusCommand, OrderResult}" />
public sealed class UpdateOrderStatusCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateOrderStatusCommand, OrderResult>
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
        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        order.ChangeStatus(request.NewStatus, DateTimeOffset.UtcNow);

        await repository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }
}
