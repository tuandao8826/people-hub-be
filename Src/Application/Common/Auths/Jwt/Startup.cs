using Application.Common.Auths.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Entities;
using Core.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application.Common.Auths.Jwt;

internal static class Startup
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		JwtTokenProfiles jwtTokenProfiles = configuration.GetRequiredSection(nameof(SecuritySettings)).Get<SecuritySettings>()!.JwtTokenProfiles;
		var adminValidation = CreateValidationParameters(jwtTokenProfiles.Admin, "Admin");
		var publicValidation = CreateValidationParameters(jwtTokenProfiles.Public, "Public");

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
						var tokenId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
						var now = DateTimeOffset.UtcNow;
						if (!Guid.TryParse(tokenId, out var sessionId) ||
							!await unitOfWork.Repository<LoginSession>().AnyAsync(
								session => session.Id == sessionId && session.UserId == userId && session.ExpiresAt > now,
								context.HttpContext.RequestAborted))
						{
							context.Fail("Invalid session.");
							return;
						}

						var user = await unitOfWork.Repository<User>().GetByIdAsync(userId, context.HttpContext.RequestAborted);
						if (user?.Status != OperationStatus.Active)
						{
							context.Fail("Inactive account.");
						}
					}
				};

				options.TokenValidationParameters = adminValidation;
			})
			.AddJwtBearer(JwtScheme.Public, options =>
			{
				options.TokenValidationParameters = publicValidation;
			});

		return services;
	}

	private static TokenValidationParameters CreateValidationParameters(JwtSettings settings, string profileName)
	{
		if (string.IsNullOrWhiteSpace(settings.ValidIssuer) || string.IsNullOrWhiteSpace(settings.ValidAudience))
			throw new InvalidOperationException($"Configure ValidIssuer and ValidAudience for the {profileName} JWT profile.");

		return new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = settings.ValidIssuer,
			ValidateAudience = true,
			ValidAudience = settings.ValidAudience,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
			ClockSkew = TimeSpan.Zero,
		};
	}
}
