using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public interface ICreateTopicStorage
{
    Task<Topic> CreateTopicAsync(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}
