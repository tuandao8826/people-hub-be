using Application.Common.Extensions;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Courses.Entities;

public class CourseParticipant : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid StaffMemberId { get; set; }
    //public StaffMember? StaffMember { get; set; }

    /// <summary>
    /// Trạng thái nhân viên đã hoàn thành khóa học này hay chưa
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Ngày thực tế nhân viên hoàn thành khóa học (dùng để sinh lịch sử đào tạo & tính ngày hết hạn)
    /// </summary>
    public DateTimeOffset? CompletionDate { get; set; }
}

public class CourseParticipantConfiguration : IEntityTypeConfiguration<CourseParticipant>
{
    public void Configure(EntityTypeBuilder<CourseParticipant> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();

        builder.HasOne(x => x.Course)
            .WithMany(x => x.CourseParticipants)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Đảm bảo một nhân viên chỉ tham gia 1 lần trong cùng 1 khóa học
        builder.HasIndex(x => new { x.CourseId, x.StaffMemberId })
            .IsUnique();
    }
}