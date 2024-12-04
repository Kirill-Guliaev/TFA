using TFA.Domain.Identity;

namespace TFA.Domain.Authorization;

public interface IIntentionResolver
{
}
public interface IIntentionResolver<TIntention> : IIntentionResolver
{
    bool IsAllowed(IIdentity subject, TIntention intention);    
}
