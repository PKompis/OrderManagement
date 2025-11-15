using FluentValidation;

namespace OrderManagement.Application.Orders.Commands.AssignOrder;

/// <summary>
/// Assign Order Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{AssignOrderCommand}" />
public sealed class AssignOrderCommandValidator : AbstractValidator<AssignOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignOrderCommandValidator"/> class.
    /// </summary>
    public AssignOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CourierId).NotEmpty();
    }
}
