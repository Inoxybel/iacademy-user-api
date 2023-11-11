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

        RuleFor(x => x.Users)
            .NotNull().WithMessage("Users list cannot be null.")
            .Must(UsersHaveValidDocument).WithMessage("Invalid Cpf in Users list.")
            .When(x => x.Users != null && x.Users.Any());

        RuleFor(x => x.AuthorizedTrainingIds)
            .NotNull().WithMessage("AuthorizedTrainingIds list cannot be null.")
            .ForEach(rule => rule.Must(BeAValidGUID).WithMessage("Invalid GUID in AuthorizedTrainingIds list."))
            .When(x => x.AuthorizedTrainingIds != null && x.AuthorizedTrainingIds.Any());
    }

    private bool UsersHaveValidDocument(List<UserInfo> users)
    {
        return users.All(user => ValidateDocument(user.Document));
    }

    private bool BeAValidGUID(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

    private static bool ValidateDocument(string userDocument)
    {
        if (userDocument == "*")
            return true;

        return DocumentValidator.IsValidCpf(userDocument);
    }
}
