using FluentValidation;

namespace OrderManagement.Application.Menu.Commands.UpdateMenuItem;

/// <summary>
/// Update Menu Item Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{UpdateMenuItemCommand}" />
public sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMenuItemCommandValidator"/> class.
    /// </summary>
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
