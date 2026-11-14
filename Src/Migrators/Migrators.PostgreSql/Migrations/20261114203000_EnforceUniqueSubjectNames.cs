using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261114203000_EnforceUniqueSubjectNames")]
public partial class EnforceUniqueSubjectNames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_subject_SubjectCode", "subject");

        migrationBuilder.AlterColumn<string>(
            name: "SubjectCode",
            table: "subject",
            type: "citext",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "subject",
            type: "citext",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.CreateIndex(
            name: "IX_subject_SubjectCode",
            table: "subject",
            column: "SubjectCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_subject_Name",
            table: "subject",
            column: "Name",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_subject_Name", "subject");
        migrationBuilder.DropIndex("IX_subject_SubjectCode", "subject");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "subject",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "citext");

        migrationBuilder.AlterColumn<string>(
            name: "SubjectCode",
            table: "subject",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "citext");

        migrationBuilder.CreateIndex(
            name: "IX_subject_SubjectCode",
            table: "subject",
            column: "SubjectCode",
            unique: true);
    }
}
