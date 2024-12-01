using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    public CreateTopicUseCase()
    {
        
    }

    public Task<Topic> ExecuteAsync(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
