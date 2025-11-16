using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Orders.Abstractions;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Handlers;

/// <summary>
/// Get Delivery Assignments Query Handler Tests
/// </summary>
public class GetDeliveryAssignmentsQueryHandlerTests
{
    private readonly Mock<IOrderReadRepository> _readRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private GetDeliveryAssignmentsQueryHandler CreateHandler()
        => new(_readRepositoryMock.Object, _mapperMock.Object, _currentUserMock.Object);

    [Fact]
    public async Task DeliveryUserForCurrentCourier()
    {
        var courierId = Guid.NewGuid();
        var query = new GetDeliveryAssignmentsQuery();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(courierId);
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Delivery);

        var domainOrders = new List<Domain.Orders.Entities.Order>();
        var mappedModels = new List<OrderModel>();

        _readRepositoryMock
            .Setup(r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainOrders);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders))
            .Returns(mappedModels);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Same(mappedModels, result.Orders);

        _readRepositoryMock.Verify(
            r => r.GetByFilterAsync(
                It.Is<OrderFilter>(f =>
                    f.AssignedCourierId == courierId &&
                    f.Status == OrderStatus.OutForDelivery &&
                    f.Type == OrderType.Delivery),
                null,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mapperMock.Verify(
            m => m.Map<IReadOnlyCollection<OrderModel>>(domainOrders),
            Times.Once
        );
    }

    [Fact]
    public async Task WhenUserNotAuthenticated()
    {
        var query = new GetDeliveryAssignmentsQuery();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _readRepositoryMock.Verify(
            r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UserNotDeliveryRole()
    {
        var query = new GetDeliveryAssignmentsQuery();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, CancellationToken.None)
        );

        _readRepositoryMock.Verify(
            r => r.GetByFilterAsync(
                It.IsAny<OrderFilter>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
