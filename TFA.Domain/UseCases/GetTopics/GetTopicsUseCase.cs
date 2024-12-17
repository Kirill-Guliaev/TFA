using FluentValidation;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage getTopicsStorage;

    public GetTopicsUseCase(IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage getTopicsStorage)
    {
        this.validator = validator;
        this.getTopicsStorage = getTopicsStorage;
    }

    public async Task<(IEnumerable<Topic> resources, int totalCount)> ExecuteAsync(GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        return await getTopicsStorage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}
