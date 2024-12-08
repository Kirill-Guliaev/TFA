using FluentValidation;

namespace TFA.Domain.UseCases.CreateTopic;

internal class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(c => c.ForumId)
            .NotEmpty().WithErrorCode("Empty");
        RuleFor(t=>t.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode("Empty")
            .MaximumLength(100).WithErrorCode("Too long");
    }
}

public record CreateTopicCommand(Guid ForumId, string Title);
