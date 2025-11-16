using MediatR;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Application.Common.Security;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Menu.Abstractions;
using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Application.Menu.Commands.DeleteMenuItem;

/// <summary>
/// Delete Menu Item Command Handler
/// </summary>
/// <seealso cref="IRequestHandler{DeleteMenuItemCommand, Unit}" />
public sealed class DeleteMenuItemCommandHandler(IMenuItemRepository repository, IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequest<DeleteMenuItemCommand>
{
    /// <summary>
    /// Handles the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        ValidateCurrentUser();

        var existing = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Menu item", request.Id);

        await repository.DeleteAsync(existing, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void ValidateCurrentUser()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) throw new ForbiddenException("Authentication is required to edit the menu.");

        if (!string.Equals(currentUser.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase)) throw new ForbiddenException("Only admin can edit the menu.");
    }
}