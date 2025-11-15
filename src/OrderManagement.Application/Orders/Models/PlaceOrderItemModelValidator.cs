using FluentValidation;

namespace OrderManagement.Application.Orders.Models;

/// <summary>
/// Place Order Item Model Validator
/// </summary>
/// <seealso cref="AbstractValidator{PlaceOrderItemModel}" />
public sealed class PlaceOrderItemModelValidator : AbstractValidator<PlaceOrderItemModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceOrderItemModelValidator"/> class.
    /// </summary>
    public PlaceOrderItemModelValidator()
    {
        RuleFor(x => x.MenuItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
