using Application.Common.ApiWrapper;
using Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";

            switch (ex)
            {
                case BadRequestException brException:
                    if (brException.StatusCode == HttpStatusCode.Forbidden)
                    {
                        context.Response.StatusCode = (int)brException.StatusCode;
                    }
                    ErrorResultResponse<dynamic?> brResponse = new((int)brException.StatusCode, null, brException.Message, brException.Errors);
                    await context.Response.WriteAsync(JsonSerializer.Serialize(brResponse));
                    break;
                default:
                    ErrorResultResponse<dynamic?> serverErrorResponse = new((int)HttpStatusCode.InternalServerError, null, "An unexpected error occurred.");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(serverErrorResponse));
                    break;
            }
        }
    }
}
