using FluentValidation;

namespace OrderManagement.Application.Orders.Commands.AutoAssignOrders;

/// <summary>
/// Auto Assign Orders Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{AutoAssignOrdersCommand}" />
public sealed class AutoAssignOrdersCommandValidator : AbstractValidator<AutoAssignOrdersCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoAssignOrdersCommandValidator"/> class.
    /// </summary>
    public AutoAssignOrdersCommandValidator()
    {
        RuleFor(x => x.MaxOrders).GreaterThan(0).LessThanOrEqualTo(50);
    }
}
