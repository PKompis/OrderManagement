using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.AutoAssignOrders;

namespace OrderManagement.UnitTests.Application.Orders.Validators;

/// <summary>
/// Auto Assign Orders Command Validator Tests
/// </summary>
public class AutoAssignOrdersCommandValidatorTests
{
    private readonly AutoAssignOrdersCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new AutoAssignOrdersCommand(5);

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidMaxOrders()
    {
        var cmd = new AutoAssignOrdersCommand(0);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.MaxOrders);
    }
}
