namespace TFA.API.Authentication;

public interface IAuthTokenStorage
{
    bool TryExtract(HttpContext httpContext, out string Token);

    void Store(HttpContext httpContext, string token);
}