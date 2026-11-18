using System.ComponentModel.DataAnnotations;
using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Roles.Bases.Responses.Permissions;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Commands.Create;
using Application.Modules.Roles.Commands.Delete;
using Application.Modules.Roles.Commands.Update;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Queries.Detail;
using Application.Modules.Roles.Queries.Options;
using Application.Modules.Roles.Queries.Permissions;
using Application.Modules.Roles.Queries.Search;
using Application.Modules.SystemAccounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Roles;

[Authorize(Policy = SystemAccountAccess.Policy)]
public class RolesController(IMediator mediator) : AdminBaseController
{
    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<RoleListItemResponse>>>> SearchAsync(
        [FromQuery, Range(1, int.MaxValue / 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 8,
        [FromQuery] string? keyword = null,
        CancellationToken cancellationToken = default)
        => ResultResponse(await mediator.Send(new SearchRolesQuery(page, pageSize, keyword), cancellationToken), Message<Role>.Search());

    [HttpGet("Permissions")]
    public async Task<ActionResult<SuccessResultResponse<IReadOnlyList<PermissionBaseResponse>>>> GetPermissionsAsync(CancellationToken cancellationToken)
        => ResultResponse(await mediator.Send(new GetRolePermissionsQuery(), cancellationToken), "Danh sách quyền");

    [HttpGet("Options")]
    public async Task<ActionResult<SuccessResultResponse<IReadOnlyList<RoleOptionResponse>>>> GetOptionsAsync(CancellationToken cancellationToken)
        => ResultResponse(await mediator.Send(new GetRoleOptionsQuery(), cancellationToken), "Danh sách vai trò");

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<RoleDetailResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        => ResultResponse(await mediator.Send(new GetRoleDetailQuery(id), cancellationToken), Message<Role>.Detail());

    [HttpPost]
    public async Task<ActionResult<SuccessResultResponse<RoleDetailResponse>>> CreateAsync([FromBody] CreateRoleCommand request, CancellationToken cancellationToken)
        => ResultResponse(await mediator.Send(request, cancellationToken), Message<Role>.Create());

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<RoleDetailResponse>>> UpdateAsync(
        [FromRoute] Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
        => ResultResponse(await mediator.Send(new UpdateRoleCommand(id, request), cancellationToken), Message<Role>.Update());

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        => await mediator.Send(new DeleteRoleCommand(id), cancellationToken) ? NoContent() : NotFound();
}
