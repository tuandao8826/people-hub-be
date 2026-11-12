using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Application.Common.Auths.Jwt;

public static class AdminSessionCookie
{
    public const string Name = "peoplehub.session";
    public const string Path = "/api";

    public static bool TryRead(HttpRequest request, out string? token)
    {
        token = null;
        return !request.Headers.ContainsKey(HeaderNames.Authorization)
            && request.Cookies.TryGetValue(Name, out token);
    }
}
