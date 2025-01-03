using Microsoft.Extensions.Options;
using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfiguration configuration;
    private readonly ISymmetricDecryptor decryptor;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IOptions<AuthenticationConfiguration> options)
    {
        configuration = options.Value;
        this.decryptor = decryptor;
    }

    public async Task<IIdentity> AuthenticateAsync(string authToken, CancellationToken cancellationToken)
    {
        var userIdString = await decryptor.DecryptAsync(authToken, configuration.Key, cancellationToken);
        //TODO: verify UserId
        return new User(Guid.Parse(userIdString));
    }
}
