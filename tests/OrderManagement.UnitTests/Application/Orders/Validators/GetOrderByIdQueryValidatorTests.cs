using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Queries.GetOrderById;

namespace OrderManagement.UnitTests.Application.Orders.Validators;

/// <summary>
/// Get Order ById Query Validator Tests
/// </summary>
public class GetOrderByIdQueryValidatorTests
{
    private readonly GetOrderByIdQueryValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var query = new GetOrderByIdQuery(Guid.NewGuid());

        var result = validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidOrderId()
    {
        var query = new GetOrderByIdQuery(Guid.Empty);

        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }
}
