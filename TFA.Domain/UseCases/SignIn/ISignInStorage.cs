namespace TFA.Domain.UseCases.SignIn;

public interface ISignInStorage
{
    Task<RecognisedUser?> FindUserAsync(string username, CancellationToken cancellationToken);

    Task<Guid> CreateSessionAsync(Guid userId, DateTimeOffset expirationMoment ,CancellationToken cancellationToken);
}