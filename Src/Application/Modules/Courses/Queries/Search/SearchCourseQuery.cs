using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.Courses;
using Application.Modules.Courses.Entities;
using MediatR;

namespace Application.Modules.Courses.Queries.Search;

public class SearchCourseQuery : PaginationParams, IRequest<PaginationResponse<CourseDefaultResponse>>
{
    public CourseStatus? Status { get; init; }
}
