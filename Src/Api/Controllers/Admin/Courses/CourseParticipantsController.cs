using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.CourseParticipant;
using Application.Modules.Courses.Entities;
using Application.Modules.Courses.Queries.Participants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Courses;

public class CourseParticipantsController(IMediator mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<CourseParticipantResponse>>>> SearchAsync(
        [FromQuery] SearchCourseParticipantsQuery request,
        CancellationToken cancellationToken = default)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<CourseParticipant>.Search());
    }
}
