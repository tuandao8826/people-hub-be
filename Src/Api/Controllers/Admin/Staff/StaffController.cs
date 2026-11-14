using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Staff.Bases.Requests;
using Application.Modules.Staff.Bases.Responses;
using Application.Modules.Staff.Commands;
using Application.Modules.Staff.Commands.Create;
using Application.Modules.Staff.Commands.Delete;
using Application.Modules.Staff.Commands.Update;
using Application.Modules.Staff.Entities;
using Application.Modules.Staff.Queries.Detail;
using Application.Modules.Staff.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers.Admin.Staff;

[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StaffController(IMediator mediator) : AdminBaseController
{
    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<StaffResponse>>>> ListAsync(
        [FromQuery, Range(1, int.MaxValue / 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        [FromQuery(Name = "Search.Keyword")] string? keyword = null,
        [FromQuery] int? status = null,
        CancellationToken cancellationToken = default)
    {
        var request = new SearchStaffQuery
        {
            Page = page,
            PageSize = pageSize,
            Status = status,
            Search = string.IsNullOrWhiteSpace(keyword)
                ? null
                : new PaginationParamSearch { Keyword = keyword }
        };
        var result = await mediator.Send(request, cancellationToken);
        return ResultResponse(result, Message<StaffMember>.Search());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<StaffResponse>>> DetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await mediator.Send(new GetStaffDetailQuery(id), cancellationToken);
        return member is null ? NotFound() : ResultResponse(member, Message<StaffMember>.Detail());
    }

    [HttpPost]
    public Task<ActionResult<SuccessResultResponse<StaffResponse>>> CreateAsync([FromBody] CreateStaffCommand request, CancellationToken cancellationToken)
        => SaveAsync(request, Message<StaffMember>.Create(), cancellationToken);

    [HttpPut("{id:guid}")]
    public Task<ActionResult<SuccessResultResponse<StaffResponse>>> UpdateAsync(Guid id, [FromBody] StaffRequest request, CancellationToken cancellationToken)
        => SaveAsync(new UpdateStaffCommand(id, request), Message<StaffMember>.Update(), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await mediator.Send(new DeleteStaffCommand(id), cancellationToken))
            return NotFound();

        return NoContent();
    }

    private async Task<ActionResult<SuccessResultResponse<StaffResponse>>> SaveAsync(
        IRequest<StaffResponse?> request, string message, CancellationToken cancellationToken)
    {
        try
        {
            var member = await mediator.Send(request, cancellationToken);
            return member is null ? NotFound() : ResultResponse(member, message);
        }
        catch (DuplicateStaffCodeException)
        {
            return Conflict(new ErrorResultResponse<object>(StatusCodes.Status409Conflict,
                Message<StaffMember>.AlreadyExists(member => member.Code)));
        }
    }
}
