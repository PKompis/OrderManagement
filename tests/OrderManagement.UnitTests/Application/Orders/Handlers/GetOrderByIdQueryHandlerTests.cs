using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.ValueObjects;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Get Order By Id Query Handler Tests
/// </summary>
public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderReadRepository> _readRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private GetOrderByIdQueryHandler CreateHandler()
        => new(_readRepositoryMock.Object, _mapperMock.Object, _currentUserMock.Object);

    private static Order CreatePickupOrder(Guid id, Guid customerId)
    {
        var item = OrderItem.Create(
            menuItemId: Guid.NewGuid(),
            name: "Test Item",
            unitPrice: 10m,
            quantity: 1,
            notes: null);

        var order = Order.Create(
            customerId: customerId,
            type: OrderType.Pickup,
            items: [item],
            deliveryAddress: null);

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }

    private static Order CreateDeliveryOrderWithCourier(Guid id, Guid customerId, Guid courierId)
    {
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

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }


    [Fact]
    public async Task AdminUserCanViewAnyOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var order = CreatePickupOrder(orderId, customerId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(order))
            .Returns(new OrderModel { Id = orderId });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(orderId, result.Order.Id);

        _readRepositoryMock.Verify(
            r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CustomerViewsOwnOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var order = CreatePickupOrder(orderId, customerId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(customerId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Customer);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(order))
            .Returns(new OrderModel { Id = orderId });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(orderId, result.Order.Id);
    }

    [Fact]
    public async Task DeliveryCourierViewsAssignedOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var order = CreateDeliveryOrderWithCourier(orderId, customerId, courierId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(courierId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(order))
            .Returns(new OrderModel { Id = orderId });

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);
        Assert.Equal(orderId, result.Order.Id);
    }

    [Fact]
    public async Task CustomerViewsOtherOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var anotherCustomerId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var order = CreatePickupOrder(orderId, customerId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(anotherCustomerId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Customer);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _mapperMock.Verify(
            m => m.Map<OrderModel>(It.IsAny<Order>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeliveryCourierViewsUnassignedOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var differentCourierId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        var order = CreateDeliveryOrderWithCourier(orderId, customerId, courierId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(differentCourierId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _mapperMock.Verify(
            m => m.Map<OrderModel>(It.IsAny<Order>()),
            Times.Never
        );
    }

    [Fact]
    public async Task WhenUserNotAuthenticated()
    {
        var query = new GetOrderByIdQuery(Guid.NewGuid());

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _readRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task OrderNotFound()
    {
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _mapperMock.Verify(
            m => m.Map<OrderModel>(It.IsAny<Order>()),
            Times.Never
        );
    }
}