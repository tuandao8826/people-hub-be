using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Api.Common.Swagger;

public static class Startup
{
	public static IServiceCollection AddSwaggerSetup(this IServiceCollection services, IConfiguration configuration)
    {
		services.AddOptions<SwaggerSettings>()
			.Bind(configuration.GetSection(nameof(SwaggerSettings)))
            .ValidateDataAnnotations();

        services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin API" });
			options.SwaggerDoc("Public", new OpenApiInfo { Title = "Public API v1", Version = "1" });

			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter a valid token",
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				BearerFormat = "JWT",
				Scheme = "Bearer"
			});

			options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

		return services;
	}

    public static WebApplication OpenSwagger(this WebApplication app)
    {
		SwaggerSettings swaggerSettings = app.Services.GetRequiredService<IOptions<SwaggerSettings>>().Value;

		if (swaggerSettings.IsEnabled)
		{
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Admin/swagger.json", "Admin API");
                options.SwaggerEndpoint("/swagger/Public/swagger.json", "Public API v1");
            });
        }

        return app;
    }
}
