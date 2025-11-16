using FluentValidation.TestHelper;
using OrderManagement.Application.Auth.Commands;

namespace OrderManagement.UnitTests.Application.Auth.Validators;

/// <summary>
/// Login Command Validator Tests
/// </summary>
public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator validator = new();

    [Fact]
    public void ValidCase()
    {
        var cmd = new LoginCommand(Guid.NewGuid(), true);

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidUserId()
    {
        var cmd = new LoginCommand(Guid.Empty, true);

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
