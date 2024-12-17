using FluentValidation;
using TFA.Domain.Exceptions;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsValidator : AbstractValidator<GetTopicsQuery>
{
    public GetTopicsValidator()
    {
        RuleFor(q => q.ForumId).NotEmpty()
            .WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(q => q.Take).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.Invalid);
        RuleFor(q => q.Skip).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.Invalid);

    }
}
