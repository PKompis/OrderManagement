using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.AssignOrder;

namespace OrderManagement.UnitTests.Application.Orders.Validators;

/// <summary>
/// Assign Order Command Validator Tests
/// </summary>
public class AssignOrderCommandValidatorTests
{
    private readonly AssignOrderCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new AssignOrderCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidOrderId()
    {
        var cmd = new AssignOrderCommand(Guid.Empty, Guid.NewGuid());

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void NotValidCourrierId()
    {
        var cmd = new AssignOrderCommand(Guid.NewGuid(), Guid.Empty);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CourierId);
    }
}
