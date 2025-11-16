using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Commands.AutoAssignOrders;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;
using OrderManagement.Domain.Orders.ValueObjects;
using OrderManagement.Domain.Staff.Entities;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Auto Assign Orders Command Handler
/// </summary>
public class AutoAssignOrdersCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IStaffReadRepository> _staffReadRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private AutoAssignOrdersCommandHandler CreateHandler()
        => new(
            _orderRepositoryMock.Object,
            _staffReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object
        );

    private static Staff CreateCourier(string name, bool isActive = true)
        => Staff.Create(name, StaffRole.Delivery, isActive);

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

        typeof(Order)
            .GetProperty(nameof(Order.Id))!
            .SetValue(order, id);

        return order;
    }


    [Fact]
    public async Task CouriersAndOrders()
    {
        var command = new AutoAssignOrdersCommand(MaxOrders: 5);

        var couriers = new List<Staff>
        {
            CreateCourier("Courier A"),
            CreateCourier("Courier B")
        };

        var orders = new List<Order>
        {
            CreateDeliveryOrder(Guid.NewGuid()),
            CreateDeliveryOrder(Guid.NewGuid()),
            CreateDeliveryOrder(Guid.NewGuid())
        };

        _staffReadRepositoryMock
            .Setup(r => r.GetAvailableCouriersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(couriers);

        _orderRepositoryMock
            .Setup(r => r.GetPendingAssignmentOrdersAsync(command.MaxOrders, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var mappedModels = orders
            .Select(o => new OrderModel { Id = o.Id })
            .ToList();

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(orders))
            .Returns(mappedModels);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orders.Count, result.Orders.Count);

        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Exactly(orders.Count)
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mapperMock.Verify(
            m => m.Map<IReadOnlyCollection<OrderModel>>(orders),
            Times.Once
        );
    }

    [Fact]
    public async Task NoAvailableCouriers()
    {
        var command = new AutoAssignOrdersCommand(MaxOrders: 5);

        _staffReadRepositoryMock
            .Setup(r => r.GetAvailableCouriersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.GetPendingAssignmentOrdersAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CouriersButNoOrders()
    {
        var command = new AutoAssignOrdersCommand(MaxOrders: 5);

        var couriers = new List<Staff>
        {
            CreateCourier("Courier A"),
            CreateCourier("Courier B")
        };

        _staffReadRepositoryMock
            .Setup(r => r.GetAvailableCouriersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(couriers);

        _orderRepositoryMock
            .Setup(r => r.GetPendingAssignmentOrdersAsync(command.MaxOrders, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(It.IsAny<IReadOnlyCollection<Order>>()))
            .Returns([]);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Orders);
        Assert.Empty(result.Orders);

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
