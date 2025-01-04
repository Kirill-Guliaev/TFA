using FluentAssertions;
using FluentValidation;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Domain.Tests.CreateForum;

public class CreateForumUseCaseShould
{
    private readonly Mock<ICreateForumStorage> storage;
    private readonly ISetup<ICreateForumStorage, Task<Forum>> createForumSetup;
    private readonly CreateForumUseCase sut;

    public CreateForumUseCaseShould()
    {
        var validator = new Mock<IValidator<CreateForumCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateForumCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var intentionManager = new Mock<IIntentionManager>();
        intentionManager.Setup(m => m.IsAllowed(It.IsAny<ForumIntention>()))
            .Returns(true);

        storage = new Mock<ICreateForumStorage>();
        createForumSetup = storage.Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        sut = new CreateForumUseCase(validator.Object, intentionManager.Object, storage.Object);
    }

    [Fact]
    public async Task ReturnCreatedForum()
    {
        Forum exprected = new Forum()
        {
            Id = Guid.Parse("1a00d725-6cf7-448a-9ceb-aa1edfe72af2"),
            Title = "Test title"
        };
        createForumSetup.ReturnsAsync(exprected);
        var command = new CreateForumCommand("Test title");
        var actual = await sut.ExecuteAsync(command, CancellationToken.None);
        actual.Should().Be(exprected);

        storage.Verify(s => s.CreateAsync("Test title", It.IsAny<CancellationToken>()), Times.Once);
        storage.VerifyNoOtherCalls();
    }
}
