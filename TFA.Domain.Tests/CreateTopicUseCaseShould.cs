using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly CreateTopicUseCase sut;
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistSetup;
    private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;

    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        forumExistSetup = storage.Setup(s => s.ForumExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        createTopicSetup = storage.Setup(s => s.CreateTopicAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        sut = new CreateTopicUseCase(storage.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        forumExistSetup.ReturnsAsync(false);
        var forumId = Guid.Parse("fcace81a-cba1-46e5-8b1e-1c46a4a4a05e");
        var authorId = Guid.Parse("35b72866-efc8-4cb8-be49-02d6ab72891a");
        await sut.Invoking(s => s.ExecuteAsync(forumId, "Some title", authorId, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
        storage.Verify(s => s.ForumExistAsync(forumId, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic()
    {
        forumExistSetup.ReturnsAsync(true);
        var expected = new Models.Topic();
        createTopicSetup.ReturnsAsync(expected);
        var forumId = Guid.Parse("368fd05f-1e26-47a0-9b40-66a8c1929d82");
        var userId = Guid.Parse("a3ad24dd-0c6f-420f-be05-fb08f05996dc");
        var actual = await sut.ExecuteAsync(forumId, "Title", userId, CancellationToken.None);
        actual.Should().Be(expected);
        storage.Verify(s => s.CreateTopicAsync(forumId, userId, "Title", It.IsAny<CancellationToken>()), Times.Once);
    }
}