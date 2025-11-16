using Moq;
using OrderManagement.Application.Auth.Commands;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Customers.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Staff.Abstractions;
using OrderManagement.Domain.Staff.Entities;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.UnitTests.Application.Auth.Handlers;

/// <summary>
/// Login Command Handler Tests
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly Mock<ICustomerReadRepository> _customerReadRepositoryMock = new();
    private readonly Mock<IStaffReadRepository> _staffReadRepositoryMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private LoginCommandHandler CreateHandler() =>
        new(
            _customerReadRepositoryMock.Object,
            _staffReadRepositoryMock.Object,
            _tokenServiceMock.Object
        );

    [Fact]
    public async Task CustomerExists()
    {
        var customerId = Guid.NewGuid();
        var command = new LoginCommand(customerId, false);

        _customerReadRepositoryMock
            .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        const string expectedToken = "customer-jwt-token";

        _tokenServiceMock
            .Setup(t => t.GenerateToken(customerId, ApplicationRoles.Customer))
            .Returns(new TokenResult(expectedToken, DateTimeOffset.MaxValue, ApplicationRoles.Customer));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.AccessToken);

        _customerReadRepositoryMock.Verify(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        _staffReadRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateToken(customerId, ApplicationRoles.Customer), Times.Once);
    }

    [Fact]
    public async Task StaffExists()
    {
        var staff = Staff.Create("Kate Kitchen", StaffRole.Kitchen, isActive: true);

        var command = new LoginCommand(staff.Id, true);

        _staffReadRepositoryMock
            .Setup(r => r.GetByIdAsync(staff.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(staff);

        const string expectedToken = "staff-jwt-token";
        _tokenServiceMock
            .Setup(t => t.GenerateToken(staff.Id, ApplicationRoles.Kitchen))
            .Returns(new TokenResult(expectedToken, DateTimeOffset.MaxValue, ApplicationRoles.Kitchen));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.AccessToken);

        _staffReadRepositoryMock.Verify(r => r.GetByIdAsync(staff.Id, It.IsAny<CancellationToken>()), Times.Once);
        _customerReadRepositoryMock.Verify(r => r.ExistsAsync(staff.Id, It.IsAny<CancellationToken>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateToken(staff.Id, ApplicationRoles.Kitchen), Times.Once);
    }

    [Fact]
    public async Task CustomerDoesNotExists()
    {
        var customerId = Guid.NewGuid();
        var command = new LoginCommand(customerId, false);

        _customerReadRepositoryMock
            .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

        _customerReadRepositoryMock.Verify(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StaffDoesNotExists()
    {
        var staff = Staff.Create("Kate Kitchen", StaffRole.Kitchen, isActive: true);

        var command = new LoginCommand(staff.Id, true);

        _staffReadRepositoryMock
            .Setup(r => r.GetByIdAsync(staff.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Staff?)null);

        const string expectedToken = "staff-jwt-token";
        _tokenServiceMock
            .Setup(t => t.GenerateToken(staff.Id, ApplicationRoles.Kitchen))
            .Returns(new TokenResult(expectedToken, DateTimeOffset.MaxValue, ApplicationRoles.Kitchen));

        var handler = CreateHandler();

        await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

        _staffReadRepositoryMock.Verify(r => r.GetByIdAsync(staff.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
