using FluentValidation;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Get Order By Id Query Validator
/// </summary>
/// <seealso cref="AbstractValidator{GetOrderByIdQuery}" />
public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrderByIdQueryValidator"/> class.
    /// </summary>
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
