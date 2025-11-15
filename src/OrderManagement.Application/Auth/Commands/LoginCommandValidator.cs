using FluentValidation;

namespace OrderManagement.Application.Auth.Commands;

/// <summary>
/// Login Command Validator
/// </summary>
/// <seealso cref="AbstractValidator{LoginCommand}" />
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidator"/> class.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}
