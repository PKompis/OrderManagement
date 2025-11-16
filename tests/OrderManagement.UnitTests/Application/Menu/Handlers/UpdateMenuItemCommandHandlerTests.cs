using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Commands.UpdateMenuItem;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.UnitTests.Application.Menu.Handlers;

/// <summary>
/// Update Menu Item Command Handler Tests
/// </summary>
public class UpdateMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private UpdateMenuItemCommandHandler CreateHandler()
        => new(
            _menuItemRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );

    private static UpdateMenuItemCommand CreateSampleCommand(Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Margherita Pizza",
            Price = 9.99m,
            Category = "Pizza",
            IsAvailable = true
        };

    [Fact]
    public async Task ValidCreate()
    {
        var item = MenuItem.Create("Margherita Pizza", 9.99m, "Pizza", true);

        var command = CreateSampleCommand(item.Id);

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _menuItemRepositoryMock
            .Setup(m => m.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
            
        _mapperMock
            .Setup(m => m.Map<MenuItemModel>(It.IsAny<MenuItem>()))
            .Returns(new MenuItemModel
            {
                Id = command.Id,
                Name = command.Name,
                Category = command.Category,
                Price = command.Price,
                IsAvailable = command.IsAvailable
            });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(command.Id, result.Item.Id);
        Assert.Equal(command.Name, result.Item.Name);
        Assert.Equal(command.Category, result.Item.Category);
        Assert.Equal(command.Price, result.Item.Price);
        Assert.Equal(command.IsAvailable, result.Item.IsAvailable);

        _menuItemRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
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
        var command = CreateSampleCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(false);
        _currentUserMock.SetupGet(x => x.UserId).Returns((Guid?)null);
        _currentUserMock.SetupGet(x => x.Role).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));

        _menuItemRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
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
        var command = CreateSampleCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Kitchen);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None));

        _menuItemRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
