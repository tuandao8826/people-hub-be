using System.ComponentModel.DataAnnotations;
using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Exceptions;
using Application.Common.Responses;
using Application.Modules.SystemAccounts;
using Application.Modules.SystemAccounts.Bases;
using Application.Modules.SystemAccounts.Commands;
using Application.Modules.SystemAccounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.SystemAccounts;

[Authorize(Policy = SystemAccountAccess.Policy)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class SystemAccountsController(IMediator mediator) : AdminBaseController
{
    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<SystemAccountResponse>>>> ListAsync(
        [FromQuery, Range(1, int.MaxValue / 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => ResultResponse(await mediator.Send(new SearchSystemAccountsQuery(page, pageSize), cancellationToken), "SystemAccount.List");

    [HttpPost]
    public Task<ActionResult<SuccessResultResponse<SystemAccountResponse>>> CreateAsync(
        CreateSystemAccountRequest request, CancellationToken cancellationToken)
        => SaveAsync(new SaveSystemAccountCommand(null, request), cancellationToken);

    [HttpPut("{id:guid}")]
    public Task<ActionResult<SuccessResultResponse<SystemAccountResponse>>> UpdateAsync(
        Guid id, UpdateSystemAccountRequest request, CancellationToken cancellationToken)
        => SaveAsync(new SaveSystemAccountCommand(id, request), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        => await mediator.Send(new DeleteSystemAccountCommand(id), cancellationToken) ? NoContent() : NotFound();

    private async Task<ActionResult<SuccessResultResponse<SystemAccountResponse>>> SaveAsync(
        SaveSystemAccountCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var account = await mediator.Send(command, cancellationToken);
            return account is null ? NotFound() : ResultResponse(account, "SystemAccount.Saved");
        }
        catch (BadRequestException exception)
        {
            return StatusCode((int)exception.StatusCode, new ErrorResultResponse<object>((int)exception.StatusCode, exception.Message));
        }
    }
}
