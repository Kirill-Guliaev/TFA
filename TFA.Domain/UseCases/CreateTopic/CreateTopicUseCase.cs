using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.Identity;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IIntentionManager intentionManager;
    private readonly ICreateTopicStorage createTopicStorage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(IIntentionManager intentionManager, ICreateTopicStorage createTopicStorage, IIdentityProvider identityProvider)
    {
        this.intentionManager = intentionManager;
        this.createTopicStorage = createTopicStorage;
        this.identityProvider = identityProvider;
    }

    public async Task<Topic> ExecuteAsync(Guid forumId, string title, CancellationToken cancellationToken)
    {
        intentionManager.ThrowIfForbidden(TopicIntention.Create);
        var forumExists = await createTopicStorage.ForumExistAsync(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await createTopicStorage.CreateTopicAsync(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
