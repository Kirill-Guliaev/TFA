using FluentAssertions;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Domain.Tests.CreateForum;

public class CreateForumCommandValidatorShould
{
    private readonly CreateForumCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var validCommand = new CreateForumCommand("Title");
        sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void RetunFailure_WhenCommandInvalid(CreateForumCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }


    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateForumCommand("Title");
        yield return new object[] { validCommand with { Title = string.Empty } };
        yield return new object[] { validCommand with { Title = string.Join("", Enumerable.Repeat("1", 51)) } };
    }
}
