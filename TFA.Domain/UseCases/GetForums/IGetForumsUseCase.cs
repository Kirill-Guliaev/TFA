using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetForums;

public interface IGetForumsUseCase
{
    Task<IEnumerable<Forum>> ExecuteAsync(CancellationToken cancellationToken);
}
