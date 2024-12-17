using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Storage.Storages;

internal class GetTopicsStorage : IGetTopicsStorage
{
    private readonly ForumDbContext forumDbContext;

    public GetTopicsStorage(ForumDbContext forumDbContext)
    {
        this.forumDbContext = forumDbContext;
    }

    public async Task<(IEnumerable<Domain.Models.Topic> resources, int totalCount)> GetTopics(Guid forumId, int skip, int take, CancellationToken cancellationToken)
    {
        var query = forumDbContext.Topics
            .Where(f => f.ForumId == forumId)
            .AsNoTrackingWithIdentityResolution();

        var result = await query
            .Select(t => new TFA.Domain.Models.Topic()
            {
                ForumId = forumId,
                CreatedAt = t.CreatedAt,
                Id = t.TopicId,
                Title = t.Title,
                UserId = t.UserId,
            })
            .Skip(skip)
            .Take(take)
            .ToArrayAsync(cancellationToken);
        return new(result, await query.CountAsync(cancellationToken));    
    }
}
