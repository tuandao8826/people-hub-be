using Application.Common.Auths.Configurations;
using Application.Common.Auths.Jwt;
using Application.Modules.Users.Commands.Login;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers.Admin.Users;

public partial class UsersController
{
    [HttpGet("Session/Csrf")]
    [AllowAnonymous]
    public IActionResult GetSessionCsrf([FromServices] IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }

    [HttpPost("Session")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateSessionAsync(
        [FromBody] LoginUserCommand request,
        [FromServices] IAntiforgery antiforgery,
        [FromServices] IWebHostEnvironment environment,
        [FromServices] IOptions<SecuritySettings> securityOptions,
        CancellationToken cancellationToken)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest();
        }

        var login = await mediator.Send(request, cancellationToken);
        var options = SessionCookieOptions(environment);
        options.MaxAge = TimeSpan.FromMinutes(securityOptions.Value.JwtTokenProfiles.Admin.TokenValidityInMinutes);
        Response.Cookies.Append(AdminSessionCookie.Name, login.AccessToken, options);
        return NoContent();
    }

    [HttpDelete("Session")]
    [AllowAnonymous]
    public IActionResult DeleteSession(
        [FromServices] IWebHostEnvironment environment,
        [FromServices] IOptions<AntiforgeryOptions> antiforgeryOptions)
    {
        Response.Cookies.Delete(AdminSessionCookie.Name, SessionCookieOptions(environment));

        var csrfCookie = antiforgeryOptions.Value.Cookie;
        if (csrfCookie.Name is { } csrfCookieName)
        {
            Response.Cookies.Delete(csrfCookieName, csrfCookie.Build(HttpContext));
        }

        return NoContent();
    }

    private CookieOptions SessionCookieOptions(IWebHostEnvironment environment) => new()
    {
        HttpOnly = true,
        Secure = !environment.IsDevelopment() || Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = AdminSessionCookie.Path,
    };
}
