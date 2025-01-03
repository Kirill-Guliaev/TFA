using TFA.Domain.Identity;

namespace TFA.Domain.UseCases.SignIn;

public interface ISignInUseCase
{
    Task<(IIdentity identity, string token)> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken);
}