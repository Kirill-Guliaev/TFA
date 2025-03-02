using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using System.Security.Cryptography;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.SignIn;
using Xunit.Abstractions;

namespace TFA.Domain.Tests.SignIn;

public class SignInUseCaseShould
{
    private readonly ISetup<IPasswordManager, bool> passwordManagerSetup;
    private readonly Mock<ISymmetricEncryptor> encryptor;
    private readonly SignInUseCase sut;
    private readonly Mock<ISignInStorage> storage;
    private readonly ISetup<ISignInStorage, Task<RecognisedUser?>> findUserSetup;
    private readonly ISetup<ISignInStorage, Task<Guid>> createSessionSetup;
    private readonly ISetup<ISymmetricEncryptor, Task<string>> encryptorSetup;

    public SignInUseCaseShould()
    {
        var validator = new Mock<IValidator<SignInCommand>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()));

        storage = new Mock<ISignInStorage>();
        findUserSetup = storage.Setup(s => s.FindUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        createSessionSetup = storage.Setup(s => s.CreateSessionAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()));

        var passwordManager = new Mock<IPasswordManager>();
        passwordManagerSetup = passwordManager.Setup(m => m.ComparePasswords(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()));
        encryptor = new Mock<ISymmetricEncryptor>();
        encryptorSetup = encryptor.Setup(s => s.EncryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
        var configuration = new Mock<IOptions<AuthenticationConfiguration>>();
        configuration.Setup(s => s.Value)
            .Returns(new AuthenticationConfiguration()
            {
                Base64Key = "cRrya6xj1B83gFuGQJTawC8uNBRY+gXbb+33C54pTdg="
            });
        sut = new SignInUseCase(validator.Object, storage.Object, passwordManager.Object, encryptor.Object, configuration.Object);
    }

    [Fact]
    public async Task ThrowException_WhenUserNotFound()
    {
        findUserSetup.ReturnsAsync(() => null);
        (await sut.Invoking(s => s.ExecuteAsync(new SignInCommand("login", "password"), CancellationToken.None))
            .Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Login));
    }

    [Fact]
    public async Task ThrowException_WhenPasswordIncorrect()
    {
        passwordManagerSetup.Returns(false);
        findUserSetup.ReturnsAsync(new RecognisedUser() { UserId = Guid.Parse("261bebc4-a78f-45ef-8220-a4b16e36dda1") });
        (await sut.Invoking(s => s.ExecuteAsync(new SignInCommand("login", "password"), CancellationToken.None)).Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Password));
    }

    [Fact]
    public async Task CreateSession_WhenPasswordMatches()
    {
        var userId = Guid.Parse("d7086f67-b7e7-45c7-bed7-ea3f455c75f3");
        var sessionId = Guid.Parse("702518df-fba4-480c-a5b3-52e69a22ecf2");
        findUserSetup.ReturnsAsync(new RecognisedUser() { UserId = userId });
        passwordManagerSetup.Returns(true);
        createSessionSetup.ReturnsAsync(sessionId);

        (Identity.IIdentity identity, string token) = await sut.ExecuteAsync(new SignInCommand("login", "password"), CancellationToken.None);
        storage.Verify(s => s.CreateSessionAsync(userId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReturnToken_WhenPasswordCorrect()
    {
        var userId = Guid.Parse("9ddb0dcb-292c-42f9-80d8-bb7d18f2df4a");
        var sessionId = Guid.Parse("06ffffd1-c258-41a6-aafe-cdbaa4a3744d");
        findUserSetup.ReturnsAsync(new RecognisedUser()
        {
            UserId = userId,
            PasswordHash = new byte[] { 1 },
            Salt = new byte[] { 1 }
        });
        createSessionSetup.ReturnsAsync(sessionId);
        passwordManagerSetup.Returns(true);
        encryptorSetup.ReturnsAsync("token");
        (Identity.IIdentity identity, string token) = await sut.ExecuteAsync(new SignInCommand("login", "password"), CancellationToken.None);
        token.Should().NotBeEmpty();
        identity.UserId.Should().Be(userId);
        identity.SessionId.Should().Be(sessionId);
        token.Should().Be("token");
    }

   [Fact]
   public async Task EncryptSessionIdIntoToken()
    {
        var userId = Guid.Parse("d7086f67-b7e7-45c7-bed7-ea3f455c75f3");
        var sessionId = Guid.Parse("702518df-fba4-480c-a5b3-52e69a22ecf2");
        findUserSetup.ReturnsAsync(new RecognisedUser() { UserId = userId });
        passwordManagerSetup.Returns(true);
        createSessionSetup.ReturnsAsync(sessionId);

        (Identity.IIdentity identity, string token) = await sut.ExecuteAsync(new SignInCommand("login", "password"), CancellationToken.None);
        encryptor
            .Verify(s => s.EncryptAsync(sessionId.ToString(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
