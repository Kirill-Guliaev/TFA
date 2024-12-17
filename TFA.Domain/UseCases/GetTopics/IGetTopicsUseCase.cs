using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

public interface IGetTopicsUseCase
{
    Task<(IEnumerable<Topic> resources, int totalCount)> ExecuteAsync(
        GetTopicsQuery query, 
        CancellationToken cancellationToken);
}