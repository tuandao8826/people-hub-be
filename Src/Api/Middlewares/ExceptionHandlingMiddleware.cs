using Application.Common.ApiWrapper;
using Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                    context.Response.StatusCode = (int)brException.StatusCode;
                    if (brException.InnerException is not null)
                        logger.LogError(brException, "API operation failed with an underlying exception: {Message}", brException.Message);
                    else
                        logger.LogWarning(brException, "Request failed with a handled API error: {Message}", brException.Message);
                    ErrorResultResponse<dynamic?> brResponse = new((int)brException.StatusCode, null, brException.Message, brException.Errors);
                    await context.Response.WriteAsync(JsonSerializer.Serialize(brResponse));
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
                    ErrorResultResponse<dynamic?> serverErrorResponse = new((int)HttpStatusCode.InternalServerError, null, "An unexpected error occurred.");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(serverErrorResponse));
                    break;
            }
        }
    }
}
