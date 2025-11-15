using FluentValidation;

namespace OrderManagement.Application.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Update Order Status Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{UpdateOrderStatusCommand}" />
public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrderStatusCommandValidator"/> class.
    /// </summary>
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}
