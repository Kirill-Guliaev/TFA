using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Storage;

namespace TFA.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly ForumDbContext dbContext;
    private readonly ISetup<IGuidFactory, Guid> createIdSetup;
    private readonly CreateTopicUseCase sut;

    public CreateTopicUseCaseShould()
    {
        dbContext = new ForumDbContext(new DbContextOptionsBuilder<ForumDbContext>().UseInMemoryDatabase(nameof(CreateTopicUseCaseShould)).Options);
        var guidFactory =  new Mock<IGuidFactory>();
        createIdSetup = guidFactory.Setup(f => f.Create());
        sut = new CreateTopicUseCase(dbContext, guidFactory.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        await dbContext.Forums.AddAsync(new Forum()
        {
            ForumId = Guid.Parse("f4b834f4-9d71-4cbf-97f8-d08e2b6432f9"),
            Title = "Other forum"
        });
        await dbContext.SaveChangesAsync();
        var forumId = Guid.Parse("fcace81a-cba1-46e5-8b1e-1c46a4a4a05e");
        var authorId = Guid.Parse("35b72866-efc8-4cb8-be49-02d6ab72891a");
        await sut.Invoking(s => s.ExecuteAsync(forumId, "Some title", authorId, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic()
    {
        var forumId = Guid.Parse("368fd05f-1e26-47a0-9b40-66a8c1929d82");
        var userId = Guid.Parse("a3ad24dd-0c6f-420f-be05-fb08f05996dc");
        await dbContext.Forums.AddAsync(new Forum()
        {
            ForumId = forumId,
            Title = "Existing forum"
        });
        await dbContext.Users.AddAsync(new User() { Login = "Existing User", UserId = userId });
        await dbContext.SaveChangesAsync();

        var actual = await sut.ExecuteAsync(forumId, "Title", userId, CancellationToken.None);

        var allTopics = await dbContext.Topics.ToArrayAsync();
        allTopics.Should().BeEquivalentTo(new[]
        {
            new Topic()
            {
                Title = "Title",
                UserId = userId,
                ForumId = forumId,
            }
        }, cfg => cfg.Including(t => t.ForumId).Including(t => t.UserId).Including(t => t.Title));

    }
}