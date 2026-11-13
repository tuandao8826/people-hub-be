using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.CourseParticipant;
using MediatR;

namespace Application.Modules.Courses.Queries.Participants;

public class SearchCourseParticipantsQuery : PaginationParams, IRequest<PaginationResponse<CourseParticipantResponse>>
{
    public Guid CourseId { get; init; }
}
