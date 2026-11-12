using Application.Common.Extensions;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Staff.Entities;

public enum StaffStatus
{
    Active = 1,
    OnLeave = 2
}

public class StaffMember : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public StaffStatus Status { get; set; } = StaffStatus.Active;
    public DateTimeOffset? JoinedOn { get; set; }
}

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public const string CodeIndex = "IX_staff_member_Code";

    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();
        builder.HasCitextUnique(member => member.Code);
        builder.HasIndex(member => member.Code).HasDatabaseName(CodeIndex);
        builder.Property(member => member.Name).HasMaxLength(100);
        builder.Property(member => member.Email).HasMaxLength(254);
        builder.Property(member => member.Phone).HasMaxLength(30);
        builder.Property(member => member.Department).HasMaxLength(100);
        builder.Property(member => member.JobTitle).HasMaxLength(100);
    }
}
