using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Subjects.Bases.Responses;
using Application.Modules.Subjects.Commands.Create;
using Application.Modules.Subjects.Commands.Delete;
using Application.Modules.Subjects.Commands.Update;
using Application.Modules.Subjects.Entites;
using Application.Modules.Subjects.Queries.Detail;
using Application.Modules.Subjects.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Subjects;

public partial class SubjectsController(IMediator mediator) : AdminBaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> CreateAsync([FromBody] CreateSubjectCommand request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Subject>.Create());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(new UpdateSubjectCommand(id, request), cancellationToken), Message<Subject>.Update());
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<MultipleIdentiferResponse>>> DeleteRangeAsync([FromBody] DeleteSubjectCommand request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Subject>.Delete());
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<SubjectDefaultResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(new GetSubjectDetailQuery(id), cancellationToken), Message<Subject>.Detail());
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<SubjectDefaultResponse>>>> SearchAsync([FromQuery] SearchSubjectQuery request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Subject>.Search());
    }
}
