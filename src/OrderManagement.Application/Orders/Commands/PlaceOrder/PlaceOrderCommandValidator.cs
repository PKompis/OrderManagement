using FluentValidation;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.Application.Orders.Commands.PlaceOrder;

/// <summary>
/// Place Order Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{PlaceOrderCommand}" />
public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceOrderCommandValidator"/> class.
    /// </summary>
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Items).NotNull().NotEmpty().WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).SetValidator(new PlaceOrderItemModelValidator());
        When(x => x.Type == OrderType.Delivery, () =>
        {
            RuleFor(x => x.DeliveryAddress).NotNull().WithMessage("Delivery address is required for delivery orders.");

            When(x => x.DeliveryAddress is not null, () =>
            {
                RuleFor(x => x.DeliveryAddress!.Street).NotEmpty().MaximumLength(200);
                RuleFor(x => x.DeliveryAddress!.City).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DeliveryAddress!.Zip).NotEmpty().MaximumLength(20);
            });
        });
    }
}
