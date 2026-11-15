using Application.Common.Auths.Configurations;
using Application.Common.Auths.Jwt;
using Application.Common.Auths.Services;
using Application.Modules.Users.Commands.Login;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Auths;

internal static class Startup
{
	public static IServiceCollection AddAuthenticationSetup(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<SecuritySettings>()
			.Bind(configuration.GetSection(nameof(SecuritySettings)))
			.ValidateDataAnnotations();

		services.AddJwtAuthentication(configuration);

		services.AddScoped<IJwtTokenService, JwtTokenService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddSingleton<LoginAttemptLimiter>();

		return services;
	}
}
