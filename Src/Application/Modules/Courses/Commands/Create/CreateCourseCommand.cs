using Application.Modules.Courses.Entities;
using Core.Common.Enums;
using MediatR;

namespace Application.Modules.Courses.Commands.Create;

public class CreateCourseCommand : IRequest<Guid>
{
    public string CourseCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid SubjectId { get; set; }

    /// <summary>
    /// Danh sách ID nhân viên tham gia khóa học ngay khi tạo (tùy chọn)
    /// </summary>
    public ICollection<Guid>? StaffMemberIds { get; set; }
}