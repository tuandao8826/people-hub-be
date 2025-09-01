using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Commands.Create;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Queries.Detail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Roles;

public class RolesController(IMediator mediator) : AdminBaseController
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<RoleDetailResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(new GetRoleDetailQuery(id), cancellationToken), Message<Role>.Detail());
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<RoleDetailResponse>>> CreateAsync([FromBody] CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Role>.Create());
    }
}
