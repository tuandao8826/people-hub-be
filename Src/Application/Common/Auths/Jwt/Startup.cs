using Application.Common.Auths.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Entities;
using Core.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Application.Common.Auths.Jwt;

internal static class Startup
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		JwtTokenProfiles jwtTokenProfiles = configuration.GetRequiredSection(nameof(SecuritySettings)).Get<SecuritySettings>()!.JwtTokenProfiles;

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtScheme.Admin;
				options.DefaultChallengeScheme = JwtScheme.Admin;
			})
			.AddJwtBearer(JwtScheme.Admin, options =>
			{
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						if (AdminSessionCookie.TryRead(context.Request, out var token))
						{
							context.Token = token;
						}
						return Task.CompletedTask;
					},
					OnTokenValidated = async context =>
					{
						var identification = context.Principal?.FindFirst(JwtTokenClaim.Identification)?.Value;
						if (!Guid.TryParse(identification, out var userId))
						{
							context.Fail("Invalid account.");
							return;
						}

						var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
						var user = await unitOfWork.Repository<User>().GetByIdAsync(userId, context.HttpContext.RequestAborted);
						if (user?.Status != OperationStatus.Active)
						{
							context.Fail("Inactive account.");
						}
					}
				};

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenProfiles.Admin.SecretKey)),
					ClockSkew = TimeSpan.Zero,
				};
			})
			.AddJwtBearer(JwtScheme.Public, options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenProfiles.Public.SecretKey)),
					ClockSkew = TimeSpan.Zero,
				};
			});

		return services;
	}
}
