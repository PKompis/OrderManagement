using FluentValidation;

namespace OrderManagement.Application.Menu.Commands.DeleteMenuItem;

/// <summary>
/// Delete Menu Item Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{DeleteMenuItemCommand}" />
public sealed class DeleteMenuItemCommandValidator : AbstractValidator<DeleteMenuItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteMenuItemCommandValidator"/> class.
    /// </summary>
    public DeleteMenuItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
