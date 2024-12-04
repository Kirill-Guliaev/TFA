using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public interface ICreateTopicUseCase
{
    Task<Topic> ExecuteAsync(CreateTopicCommand createTopicCommand, CancellationToken cancellationToken);
}
