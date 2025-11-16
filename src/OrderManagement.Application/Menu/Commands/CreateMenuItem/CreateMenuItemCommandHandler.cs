using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;
using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Menu.Entities;

namespace OrderManagement.Application.Menu.Commands.CreateMenuItem;

/// <summary>
/// Create Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{CreateMenuItemCommand, MenuItemResult}" />
public sealed class CreateMenuItemCommandHandler(IMenuItemRepository menuItemRepository, IMapper mapper, IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequestHandler<CreateMenuItemCommand, MenuItemResult>
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
        ValidateCurrentUser();

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

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to edit the menu.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase)) throw new ForbiddenException("Only admin can edit the menu.");
    }
}