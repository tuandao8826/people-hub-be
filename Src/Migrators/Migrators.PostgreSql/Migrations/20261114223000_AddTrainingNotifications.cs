using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261114223000_AddTrainingNotifications")]
public partial class AddTrainingNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "training_notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ScanDate = table.Column<DateOnly>(type: "date", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                EmployeeCount = table.Column<int>(type: "integer", nullable: false),
                IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table => table.PrimaryKey("PK_training_notification", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_training_notification_CreatedAt",
            table: "training_notification",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_training_notification_ScanDate_Type",
            table: "training_notification",
            columns: new[] { "ScanDate", "Type" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "training_notification");
    }
}
