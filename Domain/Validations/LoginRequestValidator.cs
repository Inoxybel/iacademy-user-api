using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required.")
            .EmailAddress().WithMessage("The Email field is not a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password field is required.")
            .MinimumLength(8).WithMessage("The password must be at least 8 characters long.");
    }
}

