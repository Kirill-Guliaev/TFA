using TFA.Domain.Authorization;
using TFA.Domain.Identity;
using TFA.Domain.UseCases.SignIn;

namespace TFA.Domain.UseCases.SignOut;

public class SignOutUseCase : ISignOutUseCase
{
    private readonly IIntentionManager intentionManager;
    private readonly IIdentityProvider identityProvider;
    private readonly ISignOutStorage signOutStorage;

    public SignOutUseCase(IIntentionManager intentionManager, IIdentityProvider identityProvider, ISignOutStorage signOutStorage)
    {
        this.intentionManager = intentionManager;
        this.identityProvider = identityProvider;
        this.signOutStorage = signOutStorage;
    }

    public async Task Execute(SignOutCommand command, CancellationToken cancellationToken)
    {
        intentionManager.ThrowIfForbidden(AccountIntention.SignOut);
        var sessiongId = identityProvider.Current.SessionId;
        await signOutStorage.RemoveSession(sessiongId, cancellationToken);
    }
}
