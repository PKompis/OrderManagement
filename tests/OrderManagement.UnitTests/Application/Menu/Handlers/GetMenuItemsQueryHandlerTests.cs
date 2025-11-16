using AutoMapper;
using Moq;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Queries.GetMenuItems;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.UnitTests.Application.Menu.Handlers;

/// <summary>
/// Get Menu Items Query Handler Tests
/// </summary>
public class GetMenuItemsQueryHandlerTests
{
    private readonly Mock<IMenuItemReadRepository> _readRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private GetMenuItemsQueryHandler CreateHandler()
        => new(_readRepositoryMock.Object, _mapperMock.Object);


    [Fact]
    public async Task ItemExists()
    {
        var query = new GetMenuItemsQuery();

        var domainItems = new List<MenuItem>
        {
            MenuItem.Create("Margherita", 8.5m, "Pizza", true),
            MenuItem.Create("Coke", 2.5m, "Drinks", true)
        };

        var mappedModels = new List<MenuItemModel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Margherita",
                    Category = "Pizza",
                    Price = 8.5m,
                    IsAvailable = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Coke",
                    Category = "Drinks",
                    Price = 2.5m,
                    IsAvailable = true
                }
            };

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainItems);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<MenuItemModel>>(domainItems))
            .Returns(mappedModels);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Count);

        _readRepositoryMock.Verify(
            r => r.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mapperMock.Verify(
            m => m.Map<IReadOnlyCollection<MenuItemModel>>(domainItems),
            Times.Once
        );
    }

    [Fact]
    public async Task NoItems()
    {
        var query = new GetMenuItemsQuery();

        var domainItems = new List<MenuItem>();

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainItems);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<MenuItemModel>>(domainItems))
            .Returns([]);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);

        _readRepositoryMock.Verify(
            r => r.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mapperMock.Verify(
            m => m.Map<IReadOnlyCollection<MenuItemModel>>(domainItems),
            Times.Once
        );
    }
}
