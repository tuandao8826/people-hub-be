using Application.Common.Auths.Jwt;

namespace Api.Common.Auths;

internal static class BrowserSessionSetup
{
    public static void AddBrowserSession(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-Token";
            options.Cookie.Name = "peoplehub.csrf";
            options.Cookie.Path = AdminSessionCookie.Path;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
        });
    }
}
