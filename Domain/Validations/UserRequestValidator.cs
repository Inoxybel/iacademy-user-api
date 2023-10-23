using CrossCutting.Helpers;
using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MinimumLength(8).WithMessage("Name cannot be less than 8 characters long.")
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

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Password and Confirm Password do not match.");

        RuleFor(x => x.CompanyRef)
            .Matches(@"^\d+$").WithMessage("The CompanyRef field can only contain digits.")
            .Must(DocumentValidator.IsValidCnpj).WithMessage("The CompanyRef field is not a valid Cnpj.")
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyRef));

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("Cpf field is required")
            .Length(11).WithMessage("Cpf field should have 11 numeric digits")
            .Matches(@"^\d+$").WithMessage("The Cpf field can only contain digits.")
            .Must(DocumentValidator.IsValidCpf).WithMessage("The Cpf field is not a valid Cpf.")
            .When(x => !string.IsNullOrWhiteSpace(x.Cpf));

        RuleFor(x => x.CellphoneNumberWithDDD)
            .NotEmpty().WithMessage("CellphoneNumberWithDDD field is required.")
            .Length(11).WithMessage("CellphoneNumberWithDDD field should have 11 numeric digits.")
            .Matches(@"^\d+$").WithMessage("The CellphoneNumberWithDDD field can only contain digits.");

        RuleFor(x => x.CellphoneNumberWithDDD)
            .Must(phone => phone.Length == 11 && phone[2] == '9').WithMessage("The third digit of CellphoneNumberWithDDD must be 9.");

        RuleFor(x => x.CellphoneNumberWithDDD)
            .Must(phone => ValidDDDs.Contains(phone[..2])).WithMessage("The first two digits of CellphoneNumberWithDDD must be a valid DDD from Brazil.");
    }

    private static readonly HashSet<string> ValidDDDs = new HashSet<string>
    {
        "11", "12", "13", "14", "15", "16", "17", "18", "19",
        "21", "22", "24", "27", "28", "31", "32", "33", "34",
        "35", "37", "38", "41", "42", "43", "44", "45", "46",
        "47", "48", "49", "51", "53", "54", "55", "61", "62",
        "63", "64", "65", "66", "67", "68", "69", "71", "73",
        "74", "75", "77", "79", "81", "82", "83", "84", "85",
        "86", "87", "88", "89", "91", "92", "93", "94", "95",
        "96", "97", "98", "99"
    };
}

