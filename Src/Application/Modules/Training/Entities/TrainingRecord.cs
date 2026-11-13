using Application.Common.Extensions;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Training.Entities;

public class TrainingRecord : BaseEntity
{
    public Guid StaffMemberId { get; set; }
    public StaffMember? StaffMember { get; set; }
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public DateTimeOffset TrainingDate { get; set; }
}

public class TrainingRecordConfiguration : IEntityTypeConfiguration<TrainingRecord>
{
    public void Configure(EntityTypeBuilder<TrainingRecord> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();
        builder.HasIndex(x => new { x.StaffMemberId, x.SubjectId }).IsUnique();
        builder.HasOne(x => x.StaffMember).WithMany()
            .HasForeignKey(x => x.StaffMemberId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Subject).WithMany()
            .HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.TrainingDate).IsRequired();
    }
}
