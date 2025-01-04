using FluentAssertions;
using FluentValidation;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOn;

namespace TFA.Domain.Tests.SignOn;

public class SignOnCommandValidatorShould
{
    private SignOnCommandValidator sut;

    public SignOnCommandValidatorShould()
    {
        sut = new SignOnCommandValidator();
    }

    [Fact]
    public void ReturnTrue_WhenCommandValid()
    {
        var validCommand = new SignOnCommand("login", "qwerty123");
        sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GeteInvalidCommands))]
    public void ReturnFalse_WhenCommandInvalid(SignOnCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> GeteInvalidCommands()
    {
        var correctCommand = new SignOnCommand("login", "qwerty123");
        yield return new[] { correctCommand with { Login = string.Empty } };
        yield return new[] { correctCommand with { Login = "   " } };
        yield return new[] { correctCommand with { Password = string.Empty } };
        yield return new[] { correctCommand with { Password = "   "} };
        yield return new[] { correctCommand with { Login = string.Join("", Enumerable.Repeat("p", 30)) } };
    }
}
