using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261114234500_AllowRepeatedTrainingNotifications")]
public partial class AllowRepeatedTrainingNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_training_notification_ScanDate_Type",
            table: "training_notification");

        migrationBuilder.CreateIndex(
            name: "IX_training_notification_ScanDate_Type",
            table: "training_notification",
            columns: new[] { "ScanDate", "Type" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_training_notification_ScanDate_Type",
            table: "training_notification");

        migrationBuilder.CreateIndex(
            name: "IX_training_notification_ScanDate_Type",
            table: "training_notification",
            columns: new[] { "ScanDate", "Type" },
            unique: true);
    }
}
