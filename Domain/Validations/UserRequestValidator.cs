using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MaximumLength(100).WithMessage("Name cannot be more than 100 characters long.");

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
            .NotEmpty().WithMessage("CompanyRef field is required.")
            .Must(BeAValidCNPJ).WithMessage("The CompanyRef field is not a valid CNPJ.");
    }

    private bool BeAValidCNPJ(string cnpj)
    {
        int[] firstMultiplier = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] secondMultiplier = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCnpj;
        string digit;
        int sum;
        int remainder;

        cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

        if (cnpj.Length != 14)
            return false;

        tempCnpj = cnpj[..12];

        sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * firstMultiplier[i];

        remainder = (sum % 11);

        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit = remainder.ToString();

        tempCnpj += digit;

        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * secondMultiplier[i];

        remainder = (sum % 11);

        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit += remainder.ToString();

        return cnpj.EndsWith(digit);
    }
}

