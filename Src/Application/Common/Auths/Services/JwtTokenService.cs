using Application.Common.Auths.Jwt;
using Application.Common.Identity.Bases;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Auths.Services;

public interface IJwtTokenService
{
	string GenerateAccessToken(JwtSettings settings, IJwtUser user, LoginSession session);

	string GenerateRefreshToken();
}

public class JwtTokenService : IJwtTokenService
{
	public string GenerateAccessToken(JwtSettings settings, IJwtUser user, LoginSession session)
	{
		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(settings.SecretKey));

		JwtSecurityToken accessToken = new
		(
			signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
			issuer: settings.ValidIssuer,
			audience: settings.ValidAudience,
			expires: session.ExpiresAt.UtcDateTime,
			claims: user.GetClaims().Append(new Claim(JwtRegisteredClaimNames.Jti, session.Id.ToString()))
		);

		return new JwtSecurityTokenHandler().WriteToken(accessToken);
	}

	public string GenerateRefreshToken()
	{
		byte[] randomNumber = new byte[32];
		using RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
		numberGenerator.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}
