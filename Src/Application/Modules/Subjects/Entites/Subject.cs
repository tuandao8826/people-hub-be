using Application.Common.Extensions;
using Application.Modules.Courses.Entities;
using Core.Bases;
using Core.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Subjects.Entites;

public class Subject : BaseEntity
{
    /// <summary>
    /// Mã môn học
    /// Ex: AV-HN-01
    /// </summary>
    public string SubjectCode { get; set; } = default!;

    /// <summary>
    /// Tên môn học
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Mô tả môn học
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Giá trị chu kỳ đào tạo định kỳ (ví dụ: 1, 2, 3)
    /// </summary>
    public int CycleValue { get; set; }

    /// <summary>
    /// Đơn vị thời gian của chu kỳ (Ngày, Tháng, Năm)
    /// </summary>
    public SubjectCycleUnit SubjectCycleUnit { get; set; }

    public OperationStatus OperationStatus { get; set; } = OperationStatus.Active;

    // Navigation Property: Một môn học có thể có nhiều khóa học được mở
    public ICollection<Course>? Courses { get; set; }
}

public enum SubjectCycleUnit
{
    Days = 1,
    Months = 2,
    Years = 3
}

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();

        builder.Property(x => x.SubjectCode)
            .IsRequired();

        builder.HasIndex(x => x.SubjectCode)
            .IsUnique();

        builder.Property(x => x.CycleValue)
            .IsRequired();

        builder.Property(x => x.SubjectCycleUnit)
            .IsRequired();
    }
}