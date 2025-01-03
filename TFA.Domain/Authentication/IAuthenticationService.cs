using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

public interface IAuthenticationService
{
    Task<IIdentity> AuthenticateAsync(string authToken, CancellationToken cancellationToken);
}