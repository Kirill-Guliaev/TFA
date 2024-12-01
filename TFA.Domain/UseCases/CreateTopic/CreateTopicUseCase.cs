using Microsoft.EntityFrameworkCore;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly Storage.ForumDbContext dbContext;
    private readonly IGuidFactory guidFactory;

    public CreateTopicUseCase(Storage.ForumDbContext forumDbContext, IGuidFactory guidFactory)
    {
        this.dbContext = forumDbContext;
        this.guidFactory = guidFactory;
    }

    public async Task<Topic> ExecuteAsync(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        var forumExists = await dbContext.Forums.AnyAsync(f => f.ForumId == forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        var res = new Storage.Topic()
        {
            TopicId = guidFactory.Create(),
            ForumId = forumId,
            Title = title,
            UserId = authorId
        };
        await dbContext.Topics.AddAsync(res, cancellationToken);
        await dbContext.SaveChangesAsync();

        return new Topic()
        {
            Title = res.Title,
            Author = res.Author.Login,
            CreatedAd = res.CreatedAt,
            Id = res.TopicId
        };


    }
}
