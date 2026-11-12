using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.Courses;
using Application.Modules.Courses.Commands.Create;
using Application.Modules.Courses.Commands.Delete;
using Application.Modules.Courses.Commands.Update;
using Application.Modules.Courses.Entities;
using Application.Modules.Courses.Queries.Detail;
using Application.Modules.Courses.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Courses;

public partial class CoursesController(IMediator mediator) : AdminBaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> CreateAsync([FromForm] CreateCourseCommand request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Course>.Create());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> UpdateAsync([FromRoute] Guid id, [FromForm] UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(new UpdateCourseCommand(id, request), cancellationToken), Message<Course>.Update());
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<MultipleIdentiferResponse>>> DeleteRangeAsync([FromBody] DeleteCourseCommand request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Course>.Delete());
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<CourseDefaultResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(new GetCourseDetailQuery(id), cancellationToken), Message<Course>.Detail());
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<CourseDefaultResponse>>>> SearchAsync([FromQuery] SearchCourseQuery request, CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Course>.Search());
    }
}