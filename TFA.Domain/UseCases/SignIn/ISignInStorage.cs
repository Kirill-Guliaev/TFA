namespace TFA.Domain.UseCases.SignIn;

public interface ISignInStorage
{
    Task<RecognisedUser> FindUserAsync(string username, CancellationToken cancellationToken);
}