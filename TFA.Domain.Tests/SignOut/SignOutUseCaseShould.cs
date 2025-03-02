using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Identity;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOut;

namespace TFA.Domain.Tests.SignOut;

public class SignOutUseCaseShould
{
    private readonly Mock<ISignOutStorage> storage;
    private readonly ISetup<ISignOutStorage, Task> removeSessionSetup;
    private readonly ISetup<IIdentityProvider, IIdentity> currentIdentitySetup;
    private readonly Mock<IIntentionManager> intentionManager;
    private readonly ISetup<IIntentionManager, bool> signOutIsAllowedSetup;
    private readonly SignOutUseCase sut;

    public SignOutUseCaseShould()
    {
        storage = new Mock<ISignOutStorage>();
        removeSessionSetup = storage.Setup(s => s.RemoveSession(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        Mock<IIdentityProvider> identityProvider = new Mock<IIdentityProvider>();
        currentIdentitySetup = identityProvider.Setup(p => p.Current);
        intentionManager = new Mock<IIntentionManager>();
        signOutIsAllowedSetup = intentionManager.Setup(s => s.IsAllowed(It.IsAny<AccountIntention>()));
        sut = new SignOutUseCase(intentionManager.Object, identityProvider.Object, storage.Object);
    }

    [Fact]
    public async Task ThrowIntentionEception_WhenUserIsNotAuthenticated()
    {
        signOutIsAllowedSetup.Returns(false);
        await sut.Invoking(s => s.Execute(new SignOutCommand(), CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();
    }

    [Fact]
    public async Task RemoveCurrentIdentitySession()
    {
        var sessionId = Guid.Parse("3b59c51b-d9c0-4fcf-9591-bf73f90e0464");
        signOutIsAllowedSetup.Returns(true);
        currentIdentitySetup.Returns(new User(Guid.Empty, sessionId));
        removeSessionSetup.Returns(Task.CompletedTask);
        await sut.Execute(new SignOutCommand(), CancellationToken.None);
        storage.Verify(s => s.RemoveSession(sessionId, It.IsAny<CancellationToken>()), Times.Once);
        storage.VerifyNoOtherCalls();
    }
}
