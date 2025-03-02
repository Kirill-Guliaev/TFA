namespace TFA.Domain.Authentication;

public interface IAuthenticationStorage
{
    Task<Session?> FindSessionAsync(Guid sessionId, CancellationToken cancellationToken);
}
