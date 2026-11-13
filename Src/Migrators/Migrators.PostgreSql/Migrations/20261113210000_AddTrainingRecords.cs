using System;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261113210000_AddTrainingRecords")]
public partial class AddTrainingRecords : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "training_record",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                TrainingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_training_record", x => x.Id);
                table.ForeignKey("FK_training_record_staff_member_StaffMemberId", x => x.StaffMemberId,
                    "staff_member", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_training_record_subject_SubjectId", x => x.SubjectId,
                    "subject", "Id", onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex("IX_training_record_StaffMemberId_SubjectId", "training_record",
            new[] { "StaffMemberId", "SubjectId" }, unique: true);
        migrationBuilder.CreateIndex("IX_training_record_SubjectId", "training_record", "SubjectId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("training_record");
}
