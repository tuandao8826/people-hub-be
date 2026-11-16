using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261116010000_AddFlightSchedules")]
public sealed class AddFlightSchedules : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "flight_schedule", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
            FlightNumber = table.Column<string>(type: "citext", maxLength: 40, nullable: false),
            IcaoFlightNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
            FlightDate = table.Column<DateOnly>(type: "date", nullable: false),
            Route = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            RegistrationNumber = table.Column<string>(type: "citext", maxLength: 20, nullable: true),
            AircraftType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            FlightStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            ArrivalFlightNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
            DepartureFlightNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
            ScheduledArrivalAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            ScheduledDepartureAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            EstimatedDepartureAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            ActualLandingAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            ActualTakeoffAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            ArrivalStand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            DepartureStand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            Belt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
        }, constraints: table => table.PrimaryKey("PK_flight_schedule", x => x.Id));
        migrationBuilder.CreateIndex(name: "IX_flight_schedule_FlightDate", table: "flight_schedule", column: "FlightDate");
        migrationBuilder.CreateIndex(name: "IX_flight_schedule_FlightNumber_FlightDate_Route", table: "flight_schedule", columns: new[] { "FlightNumber", "FlightDate", "Route" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_flight_schedule_RegistrationNumber", table: "flight_schedule", column: "RegistrationNumber");
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "flight_schedule");
}
