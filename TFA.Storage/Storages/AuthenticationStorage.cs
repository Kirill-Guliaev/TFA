using TFA.Domain.Authentication;

namespace TFA.Storage.Storages;

public class AuthenticationStorage : IAuthenticationStorage
{
    public Task<Session?> FindSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
