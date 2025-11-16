using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Results;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Application.Orders.Commands.AssignOrder;

/// <summary>
/// Assign Order Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{AssignOrderCommand, OrderResult}" />
public sealed class AssignOrderCommandHandler(
    IOrderRepository orderRepository,
    IStaffReadRepository staffReadRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser
) : IRequestHandler<AssignOrderCommand, OrderResult>
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
        ValidateCurrentUser();

        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken) ?? throw new NotFoundException("Order", request.OrderId);

        var staff = await staffReadRepository.GetByIdAsync(request.CourierId, cancellationToken) ?? throw new NotFoundException("Staff", request.CourierId);

        if (!staff.IsActive) throw new BadRequestException("Cannot assign order to an inactive courier.");

        if (staff.Role != StaffRole.Delivery) throw new BadRequestException("Staff member is not a delivery courier.");

        order.AssignCourier(request.CourierId, DateTimeOffset.UtcNow);

        await orderRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderModel>(order);

        return new OrderResult { Order = model };
    }

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to assign orders.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase) && !string.Equals(currentUser.Role, ApplicationRoles.Kitchen, StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Only kitchen staff and administrators can assign orders.");
    }
}
