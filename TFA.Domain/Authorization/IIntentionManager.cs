using TFA.Domain.Identity;

namespace TFA.Domain.Authorization;

public interface IIntentionManager
{
    bool IsAllowed<TIntetion>(TIntetion intetion) where TIntetion : struct;

    bool IsAllowed<TIntention, TObject>(TIntention intetion, TObject target) where TIntention : struct;
}

public class IntentionManager : IIntentionManager
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

    public bool IsAllowed<TIntention, TObject>(TIntention intetion, TObject target) where TIntention : struct
    {
        throw new NotImplementedException();
    }
}

public static class IntetionManagerExtension
{
    public static void ThrowIfForbidden<TIntetion>(this IIntentionManager intentionManager, TIntetion intetion)
        where TIntetion:struct
    {
        if(!intentionManager.IsAllowed(intetion))
        {
            throw new IntentionManagerException();
        }
    }
}