using FluentAssertions;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class PasswordManagerShould
{
    private readonly PasswordManager sut = new();
    private static readonly byte[] emptySalt = Enumerable.Repeat((byte)0, 100).ToArray();
    private static readonly byte[] emptyHash = Enumerable.Repeat((byte)0, 32).ToArray();

    [Theory]
    [InlineData("password")]
    [InlineData("qwerty123")]
    public void GenerateMeaningfullSaltAndHash(string password)
    {
        (byte[] Salt, byte[] Hash) = sut.GeneratePasswordParts(password);
        Salt.Should().HaveCount(100).And.NotBeEquivalentTo(emptySalt);
        Hash.Should().HaveCount(32).And.NotBeEquivalentTo(emptyHash);
    }

    [Fact]
    public void ReturnTrue_WhenPasswordMatch()
    {
        var password = "qwerty123";
        (byte[] Salt, byte[] Hash) = sut.GeneratePasswordParts(password);
        sut.ComparePasswords(password, Salt, Hash).Should().BeTrue();
    }

    [Fact]
    public void ReturnFalse_WhenPasswordDoesntMatch()
    {
        var password = "qwerty123";
        (byte[] Salt, byte[] Hash) = sut.GeneratePasswordParts(password);
        sut.ComparePasswords("password", Salt, Hash).Should().BeFalse();
    }
}
