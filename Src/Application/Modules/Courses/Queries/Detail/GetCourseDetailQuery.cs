using Application.Modules.Courses.Bases.Responses.Courses;
using MediatR;

namespace Application.Modules.Courses.Queries.Detail;

public record class GetCourseDetailQuery(Guid CourseId) : IRequest<CourseDefaultResponse>;