using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.Courses;
using MediatR;

namespace Application.Modules.Courses.Queries.Search;

public class SearchCourseQuery : PaginationParams, IRequest<PaginationResponse<CourseDefaultResponse>>;
