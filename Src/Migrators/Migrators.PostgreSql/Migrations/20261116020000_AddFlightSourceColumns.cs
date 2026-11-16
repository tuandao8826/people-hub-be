using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrators.PostgreSql.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261116020000_AddFlightSourceColumns")]
public sealed class AddFlightSourceColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateOnly>("ArrivalFlightDate", "flight_schedule", type: "date", nullable: true);
        migrationBuilder.AddColumn<DateOnly>("DepartureFlightDate", "flight_schedule", type: "date", nullable: true);
        migrationBuilder.AddColumn<string>("ArrivalRouteIcao", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("DepartureRouteIcao", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("HandlingAgent", "flight_schedule", type: "character varying(40)", maxLength: 40, nullable: true);
        migrationBuilder.AddColumn<string>("LandingRunway", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("TakeoffRunway", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("NatureOfFlight", "flight_schedule", type: "character varying(40)", maxLength: 40, nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("EstimatedInBlockAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<string>("EstimatedTaxiTime", "flight_schedule", type: "character varying(30)", maxLength: 30, nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("TargetOffBlockAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("CalculatedTakeoffAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("TargetStartupApprovalAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualStartupApprovalAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualGroundHandlingAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualOffBlockAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualInBlockAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualRunwayDepartureAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualEngineStartAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualStartupAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("TargetTaxiTime", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<string>("ArrivalGate", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<string>("DepartureGate", "flight_schedule", type: "character varying(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("EstimatedLandingAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("ActualStartRequestAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<string>("SourceRowIndex", "flight_schedule", type: "character varying(30)", maxLength: 30, nullable: true);
        migrationBuilder.AddColumn<string>("ArrivalIcaoFlightNumber", "flight_schedule", type: "character varying(40)", maxLength: 40, nullable: true);
        migrationBuilder.AddColumn<string>("DepartureIcaoFlightNumber", "flight_schedule", type: "character varying(40)", maxLength: 40, nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("EstimatedDepartureAtSource", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>("EstimatedArrivalAt", "flight_schedule", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<string>("TargetOffBlockCount", "flight_schedule", type: "character varying(30)", maxLength: 30, nullable: true);
        migrationBuilder.AddColumn<string>("TargetStartupApprovalCount", "flight_schedule", type: "character varying(30)", maxLength: 30, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var column in new[] { "ArrivalFlightDate", "DepartureFlightDate", "ArrivalRouteIcao", "DepartureRouteIcao", "HandlingAgent", "LandingRunway", "TakeoffRunway", "NatureOfFlight", "EstimatedInBlockAt", "EstimatedTaxiTime", "TargetOffBlockAt", "CalculatedTakeoffAt", "TargetStartupApprovalAt", "ActualStartupApprovalAt", "ActualGroundHandlingAt", "ActualOffBlockAt", "ActualInBlockAt", "ActualRunwayDepartureAt", "ActualEngineStartAt", "ActualStartupAt", "TargetTaxiTime", "ArrivalGate", "DepartureGate", "EstimatedLandingAt", "ActualStartRequestAt", "SourceRowIndex", "ArrivalIcaoFlightNumber", "DepartureIcaoFlightNumber", "EstimatedDepartureAtSource", "EstimatedArrivalAt", "TargetOffBlockCount", "TargetStartupApprovalCount" })
            migrationBuilder.DropColumn(column, "flight_schedule");
    }
}
