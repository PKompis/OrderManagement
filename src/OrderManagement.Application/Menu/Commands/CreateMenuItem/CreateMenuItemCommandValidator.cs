using FluentValidation;

namespace OrderManagement.Application.Menu.Commands.CreateMenuItem;

/// <summary>
/// Create Menu Item Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{CreateMenuItemCommand}" />
public sealed class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMenuItemCommandValidator"/> class.
    /// </summary>
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
