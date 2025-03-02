using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfiguration configuration;
    private readonly ISymmetricDecryptor decryptor;
    private readonly IAuthenticationStorage storage;
    private readonly ILogger<AuthenticationService> logger;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IAuthenticationStorage storage,
        ILogger<AuthenticationService> logger,
        IOptions<AuthenticationConfiguration> options)
    {
        configuration = options.Value;
        this.decryptor = decryptor;
        this.storage = storage;
        this.logger = logger;
    }

    public async Task<IIdentity> AuthenticateAsync(string authToken, CancellationToken cancellationToken)
    {
        string sessionIdString;
        try
        {
            sessionIdString = await decryptor.DecryptAsync(authToken, configuration.Key, cancellationToken);
        }
        catch (CryptographicException ex)
        {
            logger.LogWarning(
                ex, 
                "Cannot decrypt auth token ");
            return User.Guest;
        }
        if (!Guid.TryParse(sessionIdString, out var sessionId))
        {
            return User.Guest;
        }
        var session = await storage.FindSessionAsync(sessionId, cancellationToken);
        if(session is null)
        {
            return User.Guest;
        }
        if(session.ExpireAt < DateTime.UtcNow)
        {
            return User.Guest;
        }
        return new User(session.UserId, sessionId);
    }
}
