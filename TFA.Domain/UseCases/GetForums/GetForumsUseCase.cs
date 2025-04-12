using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IGetForumsUseCase
{
    private readonly IGetForumsStorage getForumsStorage;
    private readonly DomainMetrics metrics;

    public GetForumsUseCase(IGetForumsStorage getForumsStorage, DomainMetrics metrics = null)
    {
        this.getForumsStorage = getForumsStorage;
        this.metrics = metrics;
    }

    public async Task<IEnumerable<Forum>> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await getForumsStorage.GetForumsAsync(cancellationToken);
            metrics.ForumsFetched(true);
            return result;
        }
        catch
        {
            metrics.ForumsFetched(false);
            throw;
        }
    }
}
