using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Commands.UpdateOrderStatus;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Update Order Status Command Handler Tests
/// </summary>
public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private UpdateOrderStatusCommandHandler CreateHandler()
        => new(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _currentUserMock.Object
        );

    private static Order CreatePickupOrder(Guid id)
    {
        var customerId = Guid.NewGuid();

        var item = OrderItem.Create(
            menuItemId: Guid.NewGuid(),
            name: "Test Item",
            unitPrice: 10m,
            quantity: 1,
            notes: null);

        var order = Order.Create(
            customerId: customerId,
            type: OrderType.Pickup,
            items: new[] { item },
            deliveryAddress: null);

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }

    private static Order CreateDeliveryOrderOutForDelivery(Guid id, Guid courierId)
    {
        var customerId = Guid.NewGuid();

        var item = OrderItem.Create(
            menuItemId: Guid.NewGuid(),
            name: "Burger",
            unitPrice: 12m,
            quantity: 1,
            notes: null);

        var address = DeliveryAddress.Create(
            street: "Street 1",
            city: "City",
            zip: "11111",
            line2: null,
            country: "Country");

        var order = Order.Create(
            customerId: customerId,
            type: OrderType.Delivery,
            items: [item],
            deliveryAddress: address);

        order.AssignCourier(courierId, DateTimeOffset.UtcNow);
        order.ChangeStatus(OrderStatus.Preparing, DateTimeOffset.UtcNow);
        order.ChangeStatus(OrderStatus.ReadyForDelivery, DateTimeOffset.UtcNow);
        order.ChangeStatus(OrderStatus.OutForDelivery, DateTimeOffset.UtcNow);

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }

    [Fact]
    public async Task AdminUserUpdatesStatus()
    {
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Preparing);

        var order = CreatePickupOrder(orderId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(order))
            .Returns(new OrderModel { Id = orderId });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(orderId, result.Order.Id);

        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(order, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task DeliveryCourierOnAssignedOrder()
    {
        var courierId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Delivered);

        var order = CreateDeliveryOrderOutForDelivery(orderId, courierId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(courierId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(order))
            .Returns(new OrderModel { Id = orderId });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(orderId, result.Order.Id);

        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(order, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UserNotAuthenticated()
    {
        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), OrderStatus.Preparing);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None));

        _orderRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task KitchenRoleNotValidTransition()
    {
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Delivered);

        var order = CreatePickupOrder(orderId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task KitchenCancellingNonPendingOrder()
    {
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Cancelled);

        var order = CreatePickupOrder(orderId);

        order.ChangeStatus(OrderStatus.Preparing, DateTimeOffset.UtcNow);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
