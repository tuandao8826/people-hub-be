using Application.Common.Extensions;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Training.Entities;

public enum TrainingNotificationType
{
    Expired = 1,
    ExpiringSoon = 2
}

public class TrainingNotification : BaseEntity
{
    public DateOnly ScanDate { get; set; }
    public TrainingNotificationType Type { get; set; }
    public int EmployeeCount { get; set; }
    public bool IsRead { get; set; }
}

public class TrainingNotificationConfiguration : IEntityTypeConfiguration<TrainingNotification>
{
    public void Configure(EntityTypeBuilder<TrainingNotification> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();
        builder.HasIndex(notification => new { notification.ScanDate, notification.Type });
        builder.HasIndex(notification => notification.CreatedAt);
        builder.Property(notification => notification.EmployeeCount).IsRequired();
        builder.Property(notification => notification.IsRead).HasDefaultValue(false);
    }
}
