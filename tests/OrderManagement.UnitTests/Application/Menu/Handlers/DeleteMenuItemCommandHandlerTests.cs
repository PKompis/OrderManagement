using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Commands.DeleteMenuItem;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.UnitTests.Application.Menu.Handlers;

/// <summary>
/// Delete Menu Item Command Handler Tests
/// </summary>
public class DeleteMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private DeleteMenuItemCommandHandler CreateHandler()
        => new(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );

    private static DeleteMenuItemCommand CreateCommand(Guid? id = null)
        => new(id ?? Guid.NewGuid());

    [Fact]
    public async Task DeleteExistingItem()
    {
        var menuItemId = Guid.NewGuid();
        var command = CreateCommand(menuItemId);

        var existingMenuItem = MenuItem.Create(
            name: "Test Item",
            price: 10.0m,
            category: "Test Category",
            isAvailable: true
        );

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMenuItem);

        var handler = CreateHandler();

        await handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(
            r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _repositoryMock.Verify(
            r => r.DeleteAsync(existingMenuItem, It.IsAny<CancellationToken>()),
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
            handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task NonAdminUser()
    {
        var command = CreateCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task ItemDoesNotExist()
    {
        var menuItemId = Guid.NewGuid();
        var command = CreateCommand(menuItemId);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(
            r => r.GetByIdAsync(menuItemId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
