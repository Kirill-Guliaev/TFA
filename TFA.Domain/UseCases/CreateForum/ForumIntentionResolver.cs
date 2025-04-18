﻿using TFA.Domain.Authorization;
using TFA.Domain.Identity;

namespace TFA.Domain.UseCases.CreateForum;

internal class ForumIntentionResolver : IIntentionResolver<ForumIntention>
{
    public bool IsAllowed(IIdentity subject, ForumIntention intention)
    {
        return intention switch
        {
            ForumIntention.Create => subject.IsAuthenticated(),
            _ => false
        };
    }
}
