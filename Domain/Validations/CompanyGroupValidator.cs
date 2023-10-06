using CrossCutting.Helpers;
using Domain.Entities;
using FluentValidation;

namespace Domain.Validations;

public class CompanyGroupValidator : AbstractValidator<CompanyGroup>
{
    public CompanyGroupValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("GroupName field is required.")
            .MaximumLength(100).WithMessage("GroupName cannot be more than 100 characters long.");

        RuleFor(x => x.UsersDocument)
            .NotNull().WithMessage("UsersDocument list cannot be null.")
            .ForEach(rule => rule.Must(ValidateDocument).WithMessage("Invalid Cpf in UsersDocument list."))
            .When(x => x.UsersDocument != null && x.UsersDocument.Count > 0);

        RuleFor(x => x.AuthorizedTrainingIds)
            .NotNull().WithMessage("AuthorizedTrainingIds list cannot be null.")
            .ForEach(rule => rule.Must(BeAValidGUID).WithMessage("Invalid GUID in AuthorizedTrainingIds list."))
            .When(x => x.AuthorizedTrainingIds != null && x.AuthorizedTrainingIds.Count > 0)
            .Must((user, authorizedTrainingIds) => authorizedTrainingIds.Count() <= user.LimitPlan)
            .WithMessage("The AuthorizedTrainingIds list count cannot be greater than LimitPlan.")
            .When(x => x.AuthorizedTrainingIds != null);

        RuleFor(x => x.LimitPlan)
            .GreaterThan(0).WithMessage("Limit plan should be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Limit plan should be less or equal than 1000");
    }

    private bool BeAValidGUID(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

    private bool ValidateDocument(string userDocument)
    {
        if (userDocument == "*")
            return true;

        return DocumentValidator.IsValidCpf(userDocument);
    }
}
