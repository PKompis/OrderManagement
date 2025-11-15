using FluentValidation;

namespace OrderManagement.Application.Orders.Queries.GetDeliveryAssignments;

/// <summary>
/// Get Delivery Assignments Query Validator
/// </summary>
/// <seealso cref="AbstractValidator{GetDeliveryAssignmentsQuery}" />
public sealed class GetDeliveryAssignmentsQueryValidator : AbstractValidator<GetDeliveryAssignmentsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDeliveryAssignmentsQueryValidator"/> class.
    /// </summary>
    public GetDeliveryAssignmentsQueryValidator()
    {
        RuleFor(x => x.CourierId).NotEmpty();
    }
}
