using FluentValidation;

namespace AiTravelAssistant.Application.Documents.Upload;

/// <summary>
/// FluentValidation validator for <see cref="UploadDocumentCommand"/>.
/// </summary>
public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    /// <summary>
    /// Initializes a new instance of <see cref="UploadDocumentCommandValidator"/> and configures the validation rules.
    /// </summary>
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .MaximumLength(10).WithMessage("Language code cannot exceed 10 characters.");

        RuleFor(x => x.UploadedBy)
            .NotEmpty().WithMessage("UploadedBy is required.");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File content is required.");
    }
}
