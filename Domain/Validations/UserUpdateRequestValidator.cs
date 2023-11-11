using CrossCutting.Helpers;
using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MaximumLength(100).WithMessage("Name cannot be more than 100 characters long.")
            .Matches("^[a-zA-Z ]*$").WithMessage("Name can only contain letters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required.")
            .EmailAddress().WithMessage("The Email field is not a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password field is required.")
            .MinimumLength(8).WithMessage("The password must be at least 8 characters long.")
            .Matches(@"[A-Z]+").WithMessage("The password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("The password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("The password must contain at least one number.")
            .Matches(@"[\W_]+").WithMessage("The password must contain at least one special character.");

        RuleFor(x => x.CompanyRef)
            .Matches(@"^\d+$").WithMessage("The CompanyRef field can only contain digits.")
            .Must(DocumentValidator.IsValidCnpj).WithMessage("The CompanyRef field is not a valid Cnpj.")
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyRef));
    }
}

