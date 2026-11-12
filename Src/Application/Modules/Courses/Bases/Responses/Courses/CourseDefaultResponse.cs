using Application.Modules.Courses.Entities;
using Application.Modules.Subjects.Bases.Responses;
using Core.Bases;
using Core.Common.Enums;

namespace Application.Modules.Courses.Bases.Responses.Courses;

public class CourseDefaultResponse : BaseEntity
{
    public string CourseCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public SubjectDefaultResponse? Subject { get; set; }
    public CourseStatus Status { get; set; }
}
