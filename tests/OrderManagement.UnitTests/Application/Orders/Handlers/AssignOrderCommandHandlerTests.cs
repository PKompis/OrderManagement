using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.ValueObjects;
using OrderManagement.Domain.Staff.Entities;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Assign Order Command Handler Tests
/// </summary>
public class AssignOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IStaffReadRepository> _staffReadRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private AssignOrderCommandHandler CreateHandler()
        => new(
            _orderRepositoryMock.Object,
            _staffReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _currentUserMock.Object);

    private static AssignOrderCommand CreateCommand(Guid? orderId = null, Guid? courierId = null)
        => new(orderId ?? Guid.NewGuid(), courierId ?? Guid.NewGuid());

    private static Order CreateDeliveryOrder(Guid id)
    {
        var customerId = Guid.NewGuid();
        var item = OrderItem.Create(
            menuItemId: Guid.NewGuid(),
            name: "Test Item",
            unitPrice: 10m,
            quantity: 1,
            notes: null);

        var address = DeliveryAddress.Create(
            street: "Test Street",
            city: "Test City",
            zip: "12345",
            line2: null,
            country: "TestCountry");

        var order = Order.Create(
            customerId: customerId,
            type: OrderType.Delivery,
            items: [item],
            deliveryAddress: address);

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }

    private static Staff CreateActiveCourier()
        => Staff.Create("Courier John", StaffRole.Delivery, isActive: true);

    [Fact]
    public async Task AdminAssignOrder()
    {
        var orderId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var command = CreateCommand(orderId, courierId);

        var order = CreateDeliveryOrder(orderId);
        var courier = CreateActiveCourier();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _staffReadRepositoryMock
            .Setup(r => r.GetByIdAsync(courierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(courier);

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
    public async Task UnauthenticatedUser()
    {
        var command = CreateCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
    [Fact]
    public async Task NonAdminOrKitchenUser()
    {
        var command = CreateCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task OrderNotFound()
    {
        var orderId = Guid.NewGuid();
        var command = CreateCommand(orderId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _staffReadRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task StaffNotFound()
    {
        var orderId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var command = CreateCommand(orderId, courierId);

        var order = CreateDeliveryOrder(orderId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _staffReadRepositoryMock
            .Setup(r => r.GetByIdAsync(courierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Staff?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task StaffInactive()
    {
        var orderId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var command = CreateCommand(orderId, courierId);

        var order = CreateDeliveryOrder(orderId);
        var courier = Staff.Create("Inactive Courier", StaffRole.Delivery, isActive: false);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _staffReadRepositoryMock
            .Setup(r => r.GetByIdAsync(courierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(courier);

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
