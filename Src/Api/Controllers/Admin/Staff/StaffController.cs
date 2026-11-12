using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Staff;
using Application.Modules.Staff.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers.Admin.Staff;

[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
public class StaffController(IStaffService staff) : AdminBaseController
{
    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<StaffResponse>>>> ListAsync(
        [FromQuery, Range(1, int.MaxValue / 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await staff.ListAsync(new PaginationParams { Page = page, PageSize = pageSize }, cancellationToken);
        return ResultResponse(result, Message<StaffMember>.Search());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<StaffResponse>>> DetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var member = await staff.GetAsync(id, cancellationToken);
        return member is null ? NotFound() : ResultResponse(member, Message<StaffMember>.Detail());
    }

    [HttpPost]
    public Task<ActionResult<SuccessResultResponse<StaffResponse>>> CreateAsync(StaffRequest request, CancellationToken cancellationToken)
        => SaveAsync(null, request, cancellationToken);

    [HttpPut("{id:guid}")]
    public Task<ActionResult<SuccessResultResponse<StaffResponse>>> UpdateAsync(Guid id, StaffRequest request, CancellationToken cancellationToken)
        => SaveAsync(id, request, cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        => await staff.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

    private async Task<ActionResult<SuccessResultResponse<StaffResponse>>> SaveAsync(
        Guid? id, StaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var member = await staff.SaveAsync(id, request, cancellationToken);
            return member is null
                ? NotFound()
                : ResultResponse(member, id.HasValue ? Message<StaffMember>.Update() : Message<StaffMember>.Create());
        }
        catch (DuplicateStaffCodeException)
        {
            return Conflict(new ErrorResultResponse<object>(StatusCodes.Status409Conflict,
                Message<StaffMember>.AlreadyExists(member => member.Code)));
        }
    }
}
