using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IGetForumsUseCase
{
    private readonly IGetForumsStorage getForumsStorage;

    public GetForumsUseCase(IGetForumsStorage getForumsStorage)
    {
        this.getForumsStorage = getForumsStorage;
    }

    public async Task<IEnumerable<Forum>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await getForumsStorage.GetForumsAsync(cancellationToken);
    }
}
