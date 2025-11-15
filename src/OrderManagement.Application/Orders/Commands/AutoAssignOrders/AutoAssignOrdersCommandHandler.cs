using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Application.Orders.Commands.AutoAssignOrders;

/// <summary>
/// Auto Assign Orders Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{AutoAssignOrdersCommand, OrdersResult}" />
public sealed class AutoAssignOrdersCommandHandler(
    IOrderReadRepository orderReadRepository,
    IOrderRepository orderRepository,
    IStaffReadRepository staffReadRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<AutoAssignOrdersCommand, OrdersResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="BadRequestException">No available delivery couriers to assign orders to.</exception>
    public async Task<OrdersResult> Handle(AutoAssignOrdersCommand request, CancellationToken cancellationToken)
    {
        var couriers = await staffReadRepository.GetAvailableCouriersAsync(cancellationToken);

        var availableCouriers = couriers?.Where(c => c.IsActive && c.Role == StaffRole.Delivery).ToList();
        if (availableCouriers is null || availableCouriers.Count == 0) throw new BadRequestException("No available delivery couriers to assign orders to.");

        var candidateOrders = await RetriveCandidateOrders(request, cancellationToken);
        if (candidateOrders is null || candidateOrders.Count == 0) return new OrdersResult { Orders = [] };

        foreach (var order in candidateOrders)
        {
            var courier = availableCouriers[Random.Shared.Next(availableCouriers.Count)];

            order.AssignCourier(courier.Id, DateTimeOffset.UtcNow);

            await orderRepository.UpdateAsync(order, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var models = mapper.Map<IReadOnlyCollection<OrderModel>>(candidateOrders);
        return new OrdersResult { Orders = models };
    }

    private async Task<List<Order>?> RetriveCandidateOrders(AutoAssignOrdersCommand request, CancellationToken cancellationToken)
    {
        var filter = new OrderFilter { Type = OrderType.Delivery };
        var allDeliveryOrders = await orderReadRepository.GetByFilterAsync(filter, request.MaxOrders, cancellationToken);

        var candidateOrders = allDeliveryOrders
            ?.Where(o =>
                o.Assignment is null &&
                (o.Status == OrderStatus.Pending || o.Status == OrderStatus.ReadyForDelivery))
            .OrderBy(o => o.CreatedAt)
            .ToList();

        return candidateOrders;
    }
}
