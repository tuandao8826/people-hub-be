using Application.Modules.Courses.Entities;
using Core.Common.Enums;
using MediatR;

namespace Application.Modules.Courses.Commands.Update;

public record class UpdateCourseCommand(Guid CourseId, UpdateCourseRequest Data = null!) : IRequest<Guid>;

public class UpdateCourseRequest
{
    public string CourseCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid SubjectId { get; set; }
    public CourseStatus Status { get; set; }
}