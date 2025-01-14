﻿namespace TFA.API.Authentication;

internal class AuthTokenStorage : IAuthTokenStorage
{
    private const string HeaderKey = "TFA-Auth-Token";

    public void Store(HttpContext httpContext, string token)
    {
        httpContext.Response.Headers[HeaderKey] = token;
    }

    public bool TryExtract(HttpContext httpContext, out string token)
    {
        if (httpContext.Response.Headers.TryGetValue(HeaderKey, out var values) && !string.IsNullOrWhiteSpace(values.FirstOrDefault()))
        {
            token = values.First();
            return true;
        }
        token = string.Empty;
        return false;
    }
}