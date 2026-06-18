using FluentValidation;

namespace AiTravelAssistant.Application.QA.AskQuestion;

/// <summary>
/// FluentValidation validator for <see cref="AskQuestionQuery"/>.
/// </summary>
public class AskQuestionQueryValidator : AbstractValidator<AskQuestionQuery>
{
    /// <summary>
    /// Initializes a new instance of <see cref="AskQuestionQueryValidator"/> and configures the validation rules.
    /// </summary>
    public AskQuestionQueryValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question cannot be empty.")
            .MaximumLength(1000).WithMessage("Question cannot exceed 1000 characters.");
    }
}
