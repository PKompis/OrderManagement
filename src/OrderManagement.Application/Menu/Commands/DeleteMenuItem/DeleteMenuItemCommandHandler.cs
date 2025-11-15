using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;

namespace OrderManagement.Application.Menu.Commands.DeleteMenuItem;

/// <summary>
/// Delete Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{DeleteMenuItemCommand, Unit}" />
public sealed class DeleteMenuItemCommandHandler(IMenuItemRepository repository, IUnitOfWork unitOfWork) : IRequest<DeleteMenuItemCommand>
{
    /// <summary>
    /// Handles the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Menu item", request.Id);

        await repository.DeleteAsync(existing, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}