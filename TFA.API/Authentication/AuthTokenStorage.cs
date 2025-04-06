namespace TFA.API.Authentication;

internal class AuthTokenStorage : IAuthTokenStorage
{
    private const string HeaderKey = "TFA-Auth-Token";

    public void Store(HttpContext httpContext, string token)
    {
        httpContext.Response.Cookies.Append(HeaderKey, token);
    }

    public bool TryExtract(HttpContext httpContext, out string token)
    {
        if (httpContext.Request.Cookies.TryGetValue(HeaderKey, out var value) && !string.IsNullOrEmpty(value))
        {
            token = value;
            return true;
        }
        token = string.Empty;
        return false;
    }
}