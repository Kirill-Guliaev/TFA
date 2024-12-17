using FluentValidation;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage getTopicsStorage;
    private readonly IGetForumsStorage getForumsStorage;

    public GetTopicsUseCase(IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage getTopicsStorage,
        IGetForumsStorage getForumsStorage )
    {
        this.validator = validator;
        this.getTopicsStorage = getTopicsStorage;
        this.getForumsStorage = getForumsStorage;
    }

    public async Task<(IEnumerable<Topic> resources, int totalCount)> ExecuteAsync(GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        await getForumsStorage.ThrowIfForumNotFount(query.ForumId, cancellationToken);
        return await getTopicsStorage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}
