using Application.Common.Auths.Jwt;
using Microsoft.AspNetCore.Antiforgery;

namespace Api.Middlewares;

internal class BrowserSessionAntiforgeryMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        var method = context.Request.Method;
        bool requiresValidation = !HttpMethods.IsGet(method)
            && !HttpMethods.IsHead(method)
            && !HttpMethods.IsOptions(method)
            && !HttpMethods.IsTrace(method)
            && AdminSessionCookie.TryRead(context.Request, out _);

        if (requiresValidation)
        {
            try
            {
                await antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
        }

        await next(context);
    }
}
