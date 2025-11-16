using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;
using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Application.Menu.Commands.UpdateMenuItem;

/// <summary>
/// Update Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{UpdateMenuItemCommand, MenuItemResult}" />
public sealed class UpdateMenuItemCommandHandler(IMenuItemRepository repository, IMapper mapper, IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequestHandler<UpdateMenuItemCommand, MenuItemResult>
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="NotFoundException">Menu item</exception>
    public async Task<MenuItemResult> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        ValidateCurrentUser();

        var existing = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Menu item", request.Id);

        existing
            .SetName(request.Name)
            .SetPrice(request.Price)
            .SetCategory(request.Category)
            .SetAvailability(request.IsAvailable);

        await repository.UpdateAsync(existing, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<MenuItemModel>(existing);

        return new MenuItemResult { Item = model };
    }

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to edit the menu.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase)) throw new ForbiddenException("Only admin can edit the menu.");
    }
}