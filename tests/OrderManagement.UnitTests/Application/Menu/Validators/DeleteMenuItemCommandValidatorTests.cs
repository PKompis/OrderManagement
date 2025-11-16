using FluentValidation.TestHelper;
using OrderManagement.Application.Menu.Commands.DeleteMenuItem;

namespace OrderManagement.UnitTests.Application.Menu.Validators;

/// <summary>
/// Delete Menu Item Command Validator Tests
/// </summary>
public class DeleteMenuItemCommandValidatorTests
{
    private readonly DeleteMenuItemCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new DeleteMenuItemCommand(Guid.NewGuid());

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidId()
    {
        var cmd = new DeleteMenuItemCommand(Guid.Empty);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
