using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public interface ICreateTopicUseCase
{
    Task<Topic> ExecuteAsync(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken);
}
