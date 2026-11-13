using Core.Bases;

namespace Application.Modules.Courses.Bases.Responses.CourseParticipant;

public class CourseParticipantResponse : BaseEntity
{
    public Guid StaffMemberId { get; set; }
    public string StaffCode { get; set; } = default!;
    public string StaffName { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletionDate { get; set; }
}
