using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Storage.Storages;

internal class CreateTopicStorage : ICreateTopicStorage
{
    private readonly ForumDbContext dbContext;

    public CreateTopicStorage(ForumDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<bool> ForumExistAsync(Guid forumId, CancellationToken cancellationToken)
    {
        return await dbContext.Forums.AnyAsync(f => f.ForumId == forumId, cancellationToken);
    }

    public async Task<Domain.Models.Topic> CreateTopicAsync(Guid forumId, Guid userId, string title, CancellationToken cancellationToken)
    {
        var topic = new Topic()
        {
            TopicId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Title = title,
            UserId = userId,
            ForumId = forumId,
        };
        await dbContext.Topics.AddAsync(topic);
        await dbContext.SaveChangesAsync();
        return await dbContext.Topics.Where(t => t.TopicId == topic.TopicId)
            .Select(t => new Domain.Models.Topic { 
                Id = t.TopicId, 
                CreatedAd = t.CreatedAt, 
                ForumId = t.ForumId, 
                Title = t.Title, 
                UserId = t.UserId 

            }).FirstAsync();

    }
}
