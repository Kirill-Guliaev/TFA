using FluentAssertions;
using System.Security.Cryptography;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class AesSymmetricEncryptorDecryptorShould
{
    private readonly AesSymmetricEncryptorDecryptor sut = new();

    [Fact]
    public async Task ReturnMeaningfullEncryptedString()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        string actual = await sut.EncryptAsync("password", key, CancellationToken.None);
        actual.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DecryptEncryptedString_WhenKeyIsSame()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        string encryptedText = await sut.EncryptAsync("password", key, CancellationToken.None);
        var actual = await sut.DecryptAsync(encryptedText, key,CancellationToken.None);
        actual.Should().Be("password");
    }

    [Fact]
    public async Task ThrowException_WhenDecriptingWithDifferentKey()
    {
        string encryptedText = await sut.EncryptAsync("password", RandomNumberGenerator.GetBytes(32), CancellationToken.None);
        await sut.Invoking(s => s.DecryptAsync(encryptedText, RandomNumberGenerator.GetBytes(32), CancellationToken.None))
            .Should().ThrowAsync<CryptographicException>();
    }
}
