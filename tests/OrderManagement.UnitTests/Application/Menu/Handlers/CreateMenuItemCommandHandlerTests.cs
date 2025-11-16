using AutoMapper;
using Moq;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Commands.CreateMenuItem;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Menu.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.UnitTests.Application.Menu.Handlers;

/// <summary>
/// Create Menu Item Command Handler Tests
/// </summary>
public class CreateMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private CreateMenuItemCommandHandler CreateHandler()
        => new(
            _menuItemRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object
        );

    private static CreateMenuItemCommand CreateSampleCommand()
        => new(
            Name: "Margherita Pizza",
            Price: 9.99m,
            Category: "Pizza",
            IsAvailable: true
        );

    [Fact]
    public async Task ValidCreate()
    {
        var command = CreateSampleCommand();

        _currentUserMock.SetupGet(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserMock.SetupGet(x => x.Role).Returns(ApplicationRoles.Admin);

        _mapperMock
            .Setup(m => m.Map<MenuItemModel>(It.IsAny<MenuItem>()))
            .Returns(new MenuItemModel
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Category = command.Category,
                Price = command.Price,
                IsAvailable = command.IsAvailable
            });

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(command.Name, result.Item.Name);
        Assert.Equal(command.Category, result.Item.Category);
        Assert.Equal(command.Price, result.Item.Price);
        Assert.Equal(command.IsAvailable, result.Item.IsAvailable);

        _menuItemRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
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
            r => r.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
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
            r => r.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
