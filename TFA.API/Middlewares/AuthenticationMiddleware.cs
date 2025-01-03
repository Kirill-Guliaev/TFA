using TFA.API.Authentication;
using TFA.Domain.Authentication;
using TFA.Domain.Identity;

namespace TFA.API.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        IAuthTokenStorage tokenStorage,
        IAuthenticationService authenticationService,
        IIdentityProvider identityProvider)
    {
        identityProvider.Current = tokenStorage.TryExtract(httpContext, out string token)
            ? await authenticationService.AuthenticateAsync(token, CancellationToken.None)
            : User.Guest;
        await next(httpContext);
    }
}
