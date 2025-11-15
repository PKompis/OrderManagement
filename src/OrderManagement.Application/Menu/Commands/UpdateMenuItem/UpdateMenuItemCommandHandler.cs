using AutoMapper;
using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Application.Menu.Models;
using OrderManagement.Application.Menu.Results;

namespace OrderManagement.Application.Menu.Commands.UpdateMenuItem;

/// <summary>
/// Update Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{UpdateMenuItemCommand, MenuItemResult}" />
public sealed class UpdateMenuItemCommandHandler(IMenuItemRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateMenuItemCommand, MenuItemResult>
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
}