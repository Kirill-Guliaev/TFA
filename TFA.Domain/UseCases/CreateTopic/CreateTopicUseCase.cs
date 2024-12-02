using TFA.Domain.Exceptions;
using TFA.Domain.Identity;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly ICreateTopicStorage createTopicStorage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(ICreateTopicStorage createTopicStorage, IIdentityProvider identityProvider)
    {
        this.createTopicStorage = createTopicStorage;
        this.identityProvider = identityProvider;
    }

    public async Task<Topic> ExecuteAsync(Guid forumId, string title, CancellationToken cancellationToken)
    {
        var forumExists = await createTopicStorage.ForumExistAsync(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await createTopicStorage.CreateTopicAsync(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
