using CrossCutting.Helpers;
using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class CompanyLoginRequestValidator : AbstractValidator<CompanyLoginRequest>
{
    public CompanyLoginRequestValidator()
    {
        RuleFor(x => x.Cnpj)
            .Matches(@"^\d+$").WithMessage("The Cnpj field can only contain digits.")
            .Must(DocumentValidator.IsValidCnpj).WithMessage("The Cnpj field is not a valid Cnpj.")
            .When(x => !string.IsNullOrWhiteSpace(x.Cnpj));
    }
}

