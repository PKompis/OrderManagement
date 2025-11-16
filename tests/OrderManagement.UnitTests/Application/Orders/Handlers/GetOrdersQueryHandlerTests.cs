using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Entities;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Get Orders Query Handler Tests
/// </summary>
public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderReadRepository> _readRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private GetOrdersQueryHandler CreateHandler()
        => new(_readRepositoryMock.Object, _mapperMock.Object, _currentUserMock.Object);


    [Fact]
    public async Task CustomerOwnOrders()
    {
        var userId = Guid.NewGuid();
        var query = new GetOrdersQuery
        {
            Status = OrderStatus.Pending,
            Type = OrderType.Pickup,
            CustomerId = userId
        };

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(userId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Customer);

        var domainOrders = new List<Order>();
        var mapped = new List<OrderModel>();

        OrderFilter? capturedFilter = null;

        _readRepositoryMock
            .Setup(r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                null,
                It.IsAny<CancellationToken>()))
            .Callback<OrderFilter, int?, CancellationToken>((f, _, _) => capturedFilter = f)
            .ReturnsAsync(domainOrders);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders))
            .Returns(mapped);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Same(mapped, result.Orders);
        Assert.NotNull(capturedFilter);
        Assert.Equal(OrderStatus.Pending, capturedFilter!.Status);
        Assert.Equal(OrderType.Pickup, capturedFilter.Type);
        Assert.Equal(userId, capturedFilter.CustomerId);
        Assert.Null(capturedFilter.AssignedCourierId);
    }

    [Fact]
    public async Task WhenDeliveryUserAssignedOrders()
    {
        var courierId = Guid.NewGuid();
        var query = new GetOrdersQuery
        {
            Status = OrderStatus.OutForDelivery,
            Type = OrderType.Delivery,
            AssignedCourierId = courierId
        };

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(courierId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        var domainOrders = new List<Order>();
        var mapped = new List<OrderModel>();

        OrderFilter? capturedFilter = null;

        _readRepositoryMock
            .Setup(r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                null,
                It.IsAny<CancellationToken>()))
            .Callback<OrderFilter, int?, CancellationToken>((f, _, _) => capturedFilter = f)
            .ReturnsAsync(domainOrders);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders))
            .Returns(mapped);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Same(mapped, result.Orders);
        Assert.NotNull(capturedFilter);
        Assert.Equal(OrderStatus.OutForDelivery, capturedFilter!.Status);
        Assert.Equal(OrderType.Delivery, capturedFilter.Type);
        Assert.Equal(courierId, capturedFilter.AssignedCourierId);
        Assert.Null(capturedFilter.CustomerId);
    }

    [Fact]
    public async Task AdminUserArbitraryFilters()
    {
        var query = new GetOrdersQuery
        {
            Status = OrderStatus.Delivered,
            Type = OrderType.Delivery,
            AssignedCourierId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        var domainOrders = new List<Order>();
        var mapped = new List<OrderModel>();

        OrderFilter? capturedFilter = null;

        _readRepositoryMock
            .Setup(r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                null,
                It.IsAny<CancellationToken>()))
            .Callback<OrderFilter, int?, CancellationToken>((f, _, _) => capturedFilter = f)
            .ReturnsAsync(domainOrders);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders))
            .Returns(mapped);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Same(mapped, result.Orders);
        Assert.NotNull(capturedFilter);
        Assert.Equal(query.Status, capturedFilter!.Status);
        Assert.Equal(query.Type, capturedFilter.Type);
        Assert.Equal(query.AssignedCourierId, capturedFilter.AssignedCourierId);
        Assert.Equal(query.CustomerId, capturedFilter.CustomerId);
    }

    [Fact]
    public async Task Handle_WhenKitchenUser_AllowsArbitraryFilters()
    {
        var query = new GetOrdersQuery
        {
            Status = OrderStatus.Preparing,
            Type = OrderType.Pickup,
            AssignedCourierId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid()
        };

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        var domainOrders = new List<Order>();
        var mapped = new List<OrderModel>();

        OrderFilter? capturedFilter = null;

        _readRepositoryMock
            .Setup(r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                null,
                It.IsAny<CancellationToken>()))
            .Callback<OrderFilter, int?, CancellationToken>((f, _, _) => capturedFilter = f)
            .ReturnsAsync(domainOrders);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders))
            .Returns(mapped);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Same(mapped, result.Orders);
        Assert.NotNull(capturedFilter);
        Assert.Equal(query.Status, capturedFilter!.Status);
        Assert.Equal(query.Type, capturedFilter.Type);
        Assert.Equal(query.AssignedCourierId, capturedFilter.AssignedCourierId);
        Assert.Equal(query.CustomerId, capturedFilter.CustomerId);
    }

    [Fact]
    public async Task UserNotAuthenticated()
    {
        var query = new GetOrdersQuery();
        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _readRepositoryMock.Verify(
            r => r.GetByFilterAsync(It.IsAny<OrderFilter>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RoleNotRecognized()
    {
        var query = new GetOrdersQuery
        {
            Status = OrderStatus.Pending,
            Type = OrderType.Pickup
        };

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns("SomeOtherRole");

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _readRepositoryMock.Verify(
            r => r.GetByFilterAsync(It.IsAny<OrderFilter>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}

