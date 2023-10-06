using CrossCutting.Helpers;
using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MaximumLength(100).WithMessage("Name cannot be more than 100 characters long.");

        RuleFor(x => x.Cnpj)
            .Matches(@"^\d+$").WithMessage("The Cnpj field can only contain digits.")
            .Must(DocumentValidator.IsValidCnpj).WithMessage("The Cnpj field is not a valid Cnpj.")
            .When(x => !string.IsNullOrWhiteSpace(x.Cnpj));

        RuleFor(x => x.Groups)
            .ForEach(rule => rule.SetValidator(new CompanyGroupValidator()));
    }
}

