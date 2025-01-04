using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.Tests.GetForums;

public class GetForumsUseCaseShould
{
    private readonly Mock<IGetForumsStorage> storage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumSetup;
    private readonly GetForumsUseCase sut;

    public GetForumsUseCaseShould()
    {
        storage = new Mock<IGetForumsStorage>();
        getForumSetup = storage.Setup(x => x.GetForumsAsync(It.IsAny<CancellationToken>()));
        sut = new GetForumsUseCase(storage.Object);
    }

    [Fact]
    public async Task ReturnForums_FromStorage()
    {
        Forum[] exprected = new Forum[]
                    {
                new() { Id = Guid.Parse("be6b582b-ddf0-433d-bbbb-1ccb747d45d0"), Title = "Test title 1" } ,
                new() { Id = Guid.Parse("fe45a6f9-998b-4fad-be0f-cac772503b63"), Title = "Test title 2" }
                    };
        getForumSetup.ReturnsAsync(exprected);
        var actual = await sut.ExecuteAsync(CancellationToken.None);
        actual.Should().BeSameAs(exprected);
        storage.Verify(s => s.GetForumsAsync(CancellationToken.None), Times.Once());
        storage.VerifyNoOtherCalls();
    }

}
