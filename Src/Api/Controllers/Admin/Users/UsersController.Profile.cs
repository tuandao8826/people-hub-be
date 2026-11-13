using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Modules.Users.Bases.Responses.Users;
using Application.Modules.Users.Entities;
using Application.Modules.Users.Queries.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers.Admin.Users;

public partial class UsersController
{
	[HttpGet("Me")]
	[Authorize]
	public async Task<ActionResult<SuccessResultResponse<UserDefaultResponse>>> ProfileAsync(CancellationToken cancellationToken = default)
	{
		var profile = await mediator.Send(new GetUserProfileQuery(), cancellationToken);
		var expiration = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
		if (expiration is not null)
		{
			Response.Headers["X-Session-Expires-At"] = expiration;
		}
		Response.Headers.CacheControl = "no-store";
		return ResultResponse(profile, Message<User>.Login());
	}
}
