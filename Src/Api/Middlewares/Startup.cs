namespace Api.Middlewares;

public static class Startup
{
	public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder builder)
	{
		return builder
			.UseMiddleware<UserMiddleware>()
			.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
