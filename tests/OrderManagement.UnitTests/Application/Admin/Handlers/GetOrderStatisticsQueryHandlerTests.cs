using AutoMapper;
using Moq;
using OrderManagement.Application.Admin.Queries;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.UnitTests.Application.Admin.Handlers;

/// <summary>
/// Get Order Statistics Query Handler Tests
/// </summary>
public class GetOrderStatisticsQueryHandlerTests
{
    private readonly Mock<IOrderReadRepository> _readRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private GetOrderStatisticsQueryHandler CreateHandler()
        => new(_readRepoMock.Object, _mapperMock.Object, _currentUserMock.Object);

    private const string AdminRole = ApplicationRoles.Admin;

    [Fact]
    public async Task AdminUser()
    {
        _currentUserMock.SetupGet(u => u.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(u => u.Role).Returns(AdminRole);

        var statsModel = new OrderStatisticsModel
        {
            TotalOrders = 10,
            TotalPickupOrders = 3,
            TotalDeliveryOrders = 7,
            DeliveredToday = 2,
            TotalRevenue = 150m
        };

        var mapped = new OrderStatisticsResult
        {
            TotalOrders = 10,
            TotalPickupOrders = 3,
            TotalDeliveryOrders = 7,
            DeliveredToday = 2,
            TotalRevenue = 150m
        };

        _readRepoMock
            .Setup(r => r.GetStatisticsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(statsModel);

        _mapperMock
            .Setup(m => m.Map<OrderStatisticsResult>(statsModel))
            .Returns(mapped);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetOrderStatisticsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalOrders);

        _readRepoMock.Verify(r => r.GetStatisticsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OrderStatisticsResult>(statsModel), Times.Once);
    }

    [Fact]
    public async Task UserNotAuthenticated()
    {
        _currentUserMock.SetupGet(u => u.IsAuthenticated).Returns(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(async () =>
            await handler.Handle(new GetOrderStatisticsQuery(), CancellationToken.None)
        );
    }

    [Fact]
    public async Task NonAdmin()
    {
        _currentUserMock.SetupGet(u => u.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(u => u.Role).Returns(ApplicationRoles.Kitchen);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(async () =>
            await handler.Handle(new GetOrderStatisticsQuery(), CancellationToken.None)
        );
    }
}
