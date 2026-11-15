using Api.Common.Auths;
using Application.Common.Auths.Configurations;
using Application.Common.Auths.Jwt;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Commands.Login;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

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
    [EnableRateLimiting(LoginRateLimiting.Policy)]
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
    public async Task<IActionResult> DeleteSessionAsync(
        [FromServices] IWebHostEnvironment environment,
        [FromServices] IOptions<AntiforgeryOptions> antiforgeryOptions,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (Request.Headers.ContainsKey(HeaderNames.Authorization) && Request.Cookies.ContainsKey(AdminSessionCookie.Name))
            return BadRequest();

        if (User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value, out var sessionId))
        {
            await unitOfWork.Repository<LoginSession>().Find(session => session.Id == sessionId)
                .ExecuteDeleteAsync(cancellationToken);
        }

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
