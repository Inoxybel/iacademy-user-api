using Domain.DTO;
using FluentValidation;

namespace Domain.Validations;

public class FeedbackRequestValidator : AbstractValidator<FeedbackRequest>
{
    public FeedbackRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name field is required.")
            .MinimumLength(8).WithMessage("Name cannot be less than 8 characters long.")
            .MaximumLength(100).WithMessage("Name cannot be more than 100 characters long.")
            .Matches("^[a-zA-Z ]*$").WithMessage("Name can only contain letters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email field is required.")
            .EmailAddress().WithMessage("The Email field is not a valid email address.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment field is required.")
            .MinimumLength(10).WithMessage("The comment must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("The comment cannot be more than 1000 characters long.");
    }
}

