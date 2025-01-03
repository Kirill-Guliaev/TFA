using TFA.Domain.Identity;

namespace TFA.Domain.UseCases.SignOn;

public interface ISignOnUseCase
{
    Task<IIdentity> ExecuteAsync(SignOnCommand command, CancellationToken cancellationToken);
}
