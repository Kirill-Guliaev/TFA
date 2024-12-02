using TFA.Domain.Exceptions;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly ICreateTopicStorage createTopicStorage;

    public CreateTopicUseCase(ICreateTopicStorage createTopicStorage)
    {
        this.createTopicStorage = createTopicStorage;
    }

    public async Task<Topic> ExecuteAsync(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        var forumExists = await createTopicStorage.ForumExistAsync(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await createTopicStorage.CreateTopicAsync(forumId, authorId, title, cancellationToken);
    }
}
