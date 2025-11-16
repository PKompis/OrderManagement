using FluentValidation.TestHelper;
using OrderManagement.Application.Menu.Commands.UpdateMenuItem;

namespace OrderManagement.UnitTests.Application.Menu.Validators;

/// <summary>
/// Update Menu Item Command Validator Tests
/// </summary>
public class UpdateMenuItemCommandValidatorTests
{
    private readonly UpdateMenuItemCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Price = 1,
            Category = "Test",
            IsAvailable = true
        };

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidId()
    {
        var cmd = new UpdateMenuItemCommand
        {
            Id = Guid.Empty,
            Name = "Test",
            Price = 1,
            Category = "Test",
            IsAvailable = true
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void NotValidName()
    {
        var cmd = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Price = 1,
            Category = "Test",
            IsAvailable = true
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NotValidPrice()
    {
        var cmd = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Price = -1,
            Category = "Test",
            IsAvailable = true
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void NotValidCategory()
    {
        var cmd = new UpdateMenuItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Price = 1,
            Category = "",
            IsAvailable = true
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }
}
