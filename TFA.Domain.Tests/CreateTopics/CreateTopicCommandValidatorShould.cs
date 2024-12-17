using FluentAssertions;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Tests.CreateTopic;

public class CreateTopicCommandValidatorShould
{
    private readonly CreateTopicCommandValidator sut;

    public CreateTopicCommandValidatorShould()
    {
        sut = new CreateTopicCommandValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenCommandIsValid()
    {
        var actual = sut.Validate(new CreateTopicCommand(Guid.Parse("766f1bdd-ab70-4c67-8a2c-4a29e760974e"), "Correct title"));
        actual.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandIsntValid(CreateTopicCommand command, string expectedInvalidPropertyName, string expectedErrorCode)
    {
        var actual = sut.Validate(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().Contain(f => f.PropertyName == expectedInvalidPropertyName && f.ErrorCode == expectedErrorCode);
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateTopicCommand(Guid.Parse("40b563d3-8ee6-42fc-8921-c1c530134a24"), "Correct title");
        yield return new object[] { validCommand with { ForumId = Guid.Empty }, nameof(CreateTopicCommand.ForumId), ValidationErrorCode.Empty };
        yield return new object[] { validCommand with { Title = "" }, nameof(CreateTopicCommand.Title), ValidationErrorCode.Empty };
        yield return new object[] { validCommand with { Title = string.Join("a", Enumerable.Range(0, 150)) }, nameof(CreateTopicCommand.Title), ValidationErrorCode.TooLong };
    }
}
