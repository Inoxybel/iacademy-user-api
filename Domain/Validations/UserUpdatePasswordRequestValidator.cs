using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class UserUpdatePasswordRequestValidator : AbstractValidator<UserUpdatePasswordRequest>
{
    public UserUpdatePasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required.")
            .EmailAddress().WithMessage("The Email field is not a valid email address.");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old password field is required.");

        RuleFor(x => x.NewPassword)
           .NotEmpty().WithMessage("New password field is required.")
           .MinimumLength(8).WithMessage("The new password must be at least 8 characters long.")
           .Matches(@"[A-Z]+").WithMessage("The new password must contain at least one uppercase letter.")
           .Matches(@"[a-z]+").WithMessage("The new password must contain at least one lowercase letter.")
           .Matches(@"[0-9]+").WithMessage("The new password must contain at least one number.")
           .Matches(@"[\W_]+").WithMessage("The new password must contain at least one special character.")
           .Equal(x => x.ConfirmPassword).WithMessage("The new password and confirmation password do not match.");
    }
}
