using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Commands.UpdateOrderStatus;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Validators;

/// <summary>
/// Update Order Status Command Validator Tests
/// </summary>
public class UpdateOrderStatusCommandValidatorTests
{
    private readonly UpdateOrderStatusCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new UpdateOrderStatusCommand(Guid.NewGuid(), OrderStatus.Pending);

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidOrderId()
    {
        var cmd = new UpdateOrderStatusCommand(Guid.Empty, OrderStatus.Pending);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }
}
