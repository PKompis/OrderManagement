using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Customers.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Commands.PlaceOrder;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Menu.Entities;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Place Order Command Handler Tests
/// </summary>
public class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IMenuItemReadRepository> _menuItemReadRepositoryMock = new();
    private readonly Mock<ICustomerReadRepository> _customerReadRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IDeliveryEtaService> _deliveryEtaServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private PlaceOrderCommandHandler CreateHandler()
        => new(
            _orderRepositoryMock.Object,
            _menuItemReadRepositoryMock.Object,
            _customerReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _deliveryEtaServiceMock.Object,
            _currentUserMock.Object
        );

    private static PlaceOrderCommand CreatePickupCommand(Guid menuItemId) => new()
    {
        Type = OrderType.Pickup,
        Items =
            [
                    new PlaceOrderItemModel
                    {
                        MenuItemId = menuItemId,
                        Quantity = 2,
                        Notes = "Extra cheese"
                    }
            ],
        DeliveryAddress = null
    };

    private static PlaceOrderCommand CreateDeliveryCommand(Guid menuItemId) => new()
    {
        Type = OrderType.Delivery,
        Items =
            [
                    new PlaceOrderItemModel
                    {
                        MenuItemId = menuItemId,
                        Quantity = 1,
                        Notes = null
                    }
            ],
        DeliveryAddress = new DeliveryAddressModel
        {
            Street = "Street 1",
            City = "City",
            Zip = "11111",
            Line2 = null,
            Country = "Country"
        }
    };

    private void SetupAuthenticatedCustomer(Guid customerId)
    {
        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(customerId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Customer);
    }

    [Fact]
    public async Task PickupOrder()
    {
        var customerId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var command = CreatePickupCommand(menuItemId);

        SetupAuthenticatedCustomer(customerId);

        _customerReadRepositoryMock
            .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var menuItem = MenuItem.Create("Pizza", 10m, "Main", isAvailable: true);

        _menuItemReadRepositoryMock
            .Setup(r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        _mapperMock
            .Setup(m => m.Map<OrderModel>(It.IsAny<Domain.Orders.Entities.Order>()))
            .Returns(new OrderModel { Id = Guid.NewGuid() });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Order);

        _customerReadRepositoryMock.Verify(
            r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _menuItemReadRepositoryMock.Verify(
            r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Domain.Orders.Entities.Order>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        // For pickup, no ETA call expected
        _deliveryEtaServiceMock.Verify(
            s => s.GetEstimateAsync(It.IsAny<Domain.Orders.ValueObjects.DeliveryAddress>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UserNotAuthenticated()
    {
        var command = CreatePickupCommand(Guid.NewGuid());

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _customerReadRepositoryMock.Verify(
            r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UserNotCustomerRole()
    {
        var command = CreatePickupCommand(Guid.NewGuid());

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _customerReadRepositoryMock.Verify(
            r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CustomerDoesNotExist()
    {
        var customerId = Guid.NewGuid();
        var command = CreatePickupCommand(Guid.NewGuid());

        SetupAuthenticatedCustomer(customerId);

        _customerReadRepositoryMock
            .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _menuItemReadRepositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Domain.Orders.Entities.Order>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task MenuItemNotFound()
    {
        var customerId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var command = CreatePickupCommand(menuItemId);

        SetupAuthenticatedCustomer(customerId);

        _customerReadRepositoryMock
            .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _menuItemReadRepositoryMock
            .Setup(r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None)
        );

        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Domain.Orders.Entities.Order>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
