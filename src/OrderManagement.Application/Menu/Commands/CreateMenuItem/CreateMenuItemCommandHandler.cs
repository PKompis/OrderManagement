using AutoMapper;
using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Application.Menu.Commands.CreateMenuItem;

/// <summary>
/// Create Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{CreateMenuItemCommand, MenuItemResult}" />
public sealed class CreateMenuItemCommandHandler(IMenuItemRepository menuItemRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<CreateMenuItemCommand, MenuItemResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    public async Task<MenuItemResult> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = MenuItem.Create(
            name: request.Name,
            price: request.Price,
            category: request.Category,
            isAvailable: request.IsAvailable
        );

        await menuItemRepository.AddAsync(menuItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<MenuItemModel>(menuItem);

        return new MenuItemResult { Item = model };
    }
}