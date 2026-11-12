using Application.Common.Extensions;
using Application.Modules.Subjects.Entites;
using Core.Bases;
using Core.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Courses.Entities;

public class Course : BaseEntity
{
    /// <summary>
    /// Mã khóa học riêng của đợt đào tạo (Ex: COURSE-AVHN-2026-01)
    /// </summary>
    public string CourseCode { get; set; } = default!;

    /// <summary>
    /// Tên khóa học
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Mô tả khóa học
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ngày bắt đầu khóa học
    /// </summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc khóa học
    /// </summary>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Môn học mà khóa học này giảng dạy (Mỗi khóa học thuộc về 1 môn học)
    /// </summary>
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public OperationStatus OperationStatus { get; set; } = OperationStatus.Active;

    // Navigation Property: Danh sách nhân viên tham gia khóa học này
    public ICollection<CourseParticipant>? CourseParticipants { get; set; }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();

        builder.Property(x => x.CourseCode)
            .IsRequired();

        builder.HasIndex(x => x.CourseCode)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        // Thiết lập quan hệ: Một Môn học có nhiều Khóa học
        builder.HasOne(x => x.Subject)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict); // Không cho xóa môn học nếu đang có khóa học liên quan
    }
}