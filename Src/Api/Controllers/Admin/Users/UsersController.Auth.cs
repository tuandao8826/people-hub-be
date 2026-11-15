using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Api.Common.Auths;
using Application.Modules.Users.Commands.Login;
using Application.Modules.Users.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers.Admin.Users;

public partial class UsersController
{
	[HttpPost("Login")]
	[EnableRateLimiting(LoginRateLimiting.Policy)]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<ActionResult<SuccessResultResponse<LoginUserResponse>>> LoginAsync([FromBody] LoginUserCommand request, CancellationToken cancellationToken = default)
	{
		return ResultResponse(await mediator.Send(request, cancellationToken), Message<User>.Login());
	}
}
