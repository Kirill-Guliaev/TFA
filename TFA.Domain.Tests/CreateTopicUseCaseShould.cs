using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using System.Security.Principal;
using TFA.Domain.Exceptions;
using TFA.Domain.Identity;
using TFA.Domain.UseCases.CreateTopic;
using IIdentity = TFA.Domain.Identity.IIdentity;

namespace TFA.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly CreateTopicUseCase sut;
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistSetup;
    private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;
    private readonly ISetup<IIdentity, Guid> getCurrentUserSetup;

    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        forumExistSetup = storage.Setup(s => s.ForumExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        createTopicSetup = storage.Setup(s => s.CreateTopicAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        var identity = new Mock<IIdentity>();
        var identityProvide = new Mock<IIdentityProvider>();
        identityProvide.Setup(s => s.Current).Returns(identity.Object);
        getCurrentUserSetup = identity.Setup(i => i.UserId);
        sut = new CreateTopicUseCase(storage.Object, identityProvide.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("fcace81a-cba1-46e5-8b1e-1c46a4a4a05e");
        forumExistSetup.ReturnsAsync(false);
        await sut.Invoking(s => s.ExecuteAsync(forumId, "Some title", CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
        storage.Verify(s => s.ForumExistAsync(forumId, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic()
    {
        var forumId = Guid.Parse("368fd05f-1e26-47a0-9b40-66a8c1929d82");
        var userId = Guid.Parse("a3ad24dd-0c6f-420f-be05-fb08f05996dc");
        var expected = new Models.Topic();
        forumExistSetup.ReturnsAsync(true);
        createTopicSetup.ReturnsAsync(expected);
        getCurrentUserSetup.Returns(userId);
        var actual = await sut.ExecuteAsync(forumId, "Title", CancellationToken.None);
        actual.Should().Be(expected);
        storage.Verify(s => s.CreateTopicAsync(forumId, userId, "Title", It.IsAny<CancellationToken>()), Times.Once);
    }
}