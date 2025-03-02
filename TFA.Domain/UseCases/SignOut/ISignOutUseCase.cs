using TFA.Domain.UseCases.SignIn;

namespace TFA.Domain.UseCases.SignOut;

public interface ISignOutUseCase
{
    Task Execute(SignOutCommand command, CancellationToken cancellationToken);
}
