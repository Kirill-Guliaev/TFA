using TFA.Domain.Identity;

namespace TFA.Domain.Authorization;

internal class IntentionManager : IIntentionManager
{
    private readonly IEnumerable<IIntentionResolver> resolvers;
    private readonly IIdentityProvider identityProvider;

    public IntentionManager(IEnumerable<IIntentionResolver> resolvers, IIdentityProvider identityProvider)
    {
        this.resolvers = resolvers;
        this.identityProvider = identityProvider;
    }

    public bool IsAllowed<TIntetion>(TIntetion intetion) where TIntetion : struct
    {
        IIntentionResolver<TIntetion>? matchingResolver = resolvers.OfType<IIntentionResolver<TIntetion>>().FirstOrDefault();
        return matchingResolver is null ? false : matchingResolver.IsAllowed(identityProvider.Current, intetion);
    }
}
