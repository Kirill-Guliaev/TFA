using FluentValidation;
using Moq;
using TFA.Domain.UseCases.GetTopics;
using Moq.Language.Flow;
using TFA.Domain.Models;
using FluentAssertions;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.Exceptions;
namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly GetTopicsUseCase sut;
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Models.Topic> resources, int totalCount)>> getTopicsSetup;
    public GetTopicsUseCaseShould()
    {
        var validator = new Mock<IValidator<GetTopicsQuery>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var getForumStorage = new Mock<IGetForumsStorage>();
        getForumsSetup = getForumStorage.Setup(s => s.GetForumsAsync(It.IsAny<CancellationToken>()));
        storage = new Mock<IGetTopicsStorage>();
        getTopicsSetup = storage.Setup(s => s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));

        sut = new GetTopicsUseCase(validator.Object, storage.Object, getForumStorage.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoForum()
    {
        var forumId = Guid.Parse("62951654-7194-440c-addf-5babf259cf7e");
        getForumsSetup.ReturnsAsync([new Forum() { Id = Guid.Parse("f2a2bebb-384e-4e67-b43d-83ace72b51ac")}]);

        var query = new GetTopicsQuery(forumId,0,1);
        await sut.Invoking(s => s.ExecuteAsync(query,CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnTopics_ExtractedFromStorage_WhenForumExists()
    {
        var forumId = Guid.Parse("965a8ac5-3459-4048-80e2-580f4a23315b");
        getForumsSetup.ReturnsAsync(new Forum[] { new Forum() { Id = Guid.Parse("965a8ac5-3459-4048-80e2-580f4a23315b") } });
        Topic[] expectedResources = new Topic[] { new() };
        int expectedTotalCount = 6;
        getTopicsSetup.ReturnsAsync((expectedResources, expectedTotalCount));
        var (actualResources, actualTotalCount) = await sut.ExecuteAsync(new GetTopicsQuery(forumId, 5, 10), CancellationToken.None);
        actualResources.Should().BeEquivalentTo(expectedResources);//https://www.youtube.com/live/Sxs4d7yIY3g?si=GUYPRddXLqwKtrkF&t=1593
        actualResources.Should().BeEquivalentTo(expectedResources);
        actualTotalCount.Should().Be(expectedTotalCount);
        storage.Verify(s => s.GetTopics(forumId, 5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
