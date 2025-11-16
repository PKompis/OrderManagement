using FluentValidation.TestHelper;
using OrderManagement.Application.Menu.Commands.CreateMenuItem;

namespace OrderManagement.UnitTests.Application.Menu.Validators;

/// <summary>
/// Create Menu Item Command Validator Tests
/// </summary>
public class CreateMenuItemCommandValidatorTests
{
    private readonly CreateMenuItemCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new CreateMenuItemCommand("Test", 1, "Test", true);

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidName()
    {
        var cmd = new CreateMenuItemCommand("", 1, "Test", true);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NotValidPrice()
    {
        var cmd = new CreateMenuItemCommand("Test", -1, "Test", true);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void NotValidCategory()
    {
        var cmd = new CreateMenuItemCommand("Test", 1, "", true);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }
}
