using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using System.Security.Cryptography;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly ISetup<IAuthenticationStorage, Task<Session?>> findUserSessionSetup;
    private readonly ISetup<ISymmetricDecryptor, Task<string>> decryptorSetup;
    private readonly AuthenticationService sut;

    public AuthenticationServiceShould()
    {
        var decryptor = new Mock<ISymmetricDecryptor>();
        var storage = new Mock<IAuthenticationStorage>();
        findUserSessionSetup = storage.Setup(s => s.FindSessionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        decryptorSetup = decryptor.Setup(s => s.DecryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
        var options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(s => s.Value)
            .Returns(new AuthenticationConfiguration { Base64Key = "cRrya6xj1B83gFuGQJTawC8uNBRY+gXbb+33C54pTdg=" });
        sut = new AuthenticationService(decryptor.Object, storage.Object, NullLogger<AuthenticationService>.Instance, options.Object);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenCannotBeDecrypted()
    {
        decryptorSetup.Throws<CryptographicException>();
        var actual = await sut.AuthenticateAsync("some string", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenIsInvalid()
    {
        decryptorSetup.ReturnsAsync("not-a-guid");
        var actual = await sut.AuthenticateAsync("some string", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenSessionNotFound()
    {
        decryptorSetup.ReturnsAsync("b0a549d5-fa26-41d3-9880-45a8900c231a");
        findUserSessionSetup.ReturnsAsync(() => null);
        var actual = await sut.AuthenticateAsync("a7d05471-0ded-449e-88ad-ef0d880666b0", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }


    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenExpire()
    {
        decryptorSetup.ReturnsAsync("9e6c4abb-3ead-459d-8a96-5a219ccfd653");
        findUserSessionSetup.ReturnsAsync(new Session(Guid.Parse("b0a549d5-fa26-41d3-9880-45a8900c231a"), DateTimeOffset.UtcNow.AddDays(-1)));
        var actual = await sut.AuthenticateAsync("some token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnIdentity_WhenSessionIsValid()
    {
        var sessionId = Guid.Parse("7943ca07-81eb-4492-baac-ed3f4b3847b8");
        var userId = Guid.Parse("ca044a36-81c9-4cfc-9c06-889854a6be4b");
        decryptorSetup.ReturnsAsync("7943ca07-81eb-4492-baac-ed3f4b3847b8");
        findUserSessionSetup.ReturnsAsync(new Session(userId, DateTimeOffset.UtcNow.AddDays(1)));
        var actual = await sut.AuthenticateAsync("some token", CancellationToken.None);
        actual.Should().BeEquivalentTo(new User(userId, sessionId));
    }
}
