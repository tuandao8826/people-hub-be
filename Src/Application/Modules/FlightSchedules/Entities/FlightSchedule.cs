using Application.Common.Extensions;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.FlightSchedules.Entities;

public class FlightSchedule : BaseEntity
{
    public string FlightNumber { get; set; } = null!;
    public string? IcaoFlightNumber { get; set; }
    public DateOnly FlightDate { get; set; }
    public string Route { get; set; } = null!;
    public string? RegistrationNumber { get; set; }
    public string? AircraftType { get; set; }
    public string? FlightStatus { get; set; }
    public string? ArrivalFlightNumber { get; set; }
    public string? DepartureFlightNumber { get; set; }
    public DateTimeOffset? ScheduledArrivalAt { get; set; }
    public DateTimeOffset? ScheduledDepartureAt { get; set; }
    public DateTimeOffset? EstimatedDepartureAt { get; set; }
    public DateTimeOffset? ActualLandingAt { get; set; }
    public DateTimeOffset? ActualTakeoffAt { get; set; }
    public string? ArrivalStand { get; set; }
    public string? DepartureStand { get; set; }
    public string? Belt { get; set; }

    // Additional source columns from the airport flight plan file.
    public DateOnly? ArrivalFlightDate { get; set; }
    public DateOnly? DepartureFlightDate { get; set; }
    public string? ArrivalRouteIcao { get; set; }
    public string? DepartureRouteIcao { get; set; }
    public string? HandlingAgent { get; set; }
    public string? LandingRunway { get; set; }
    public string? TakeoffRunway { get; set; }
    public string? NatureOfFlight { get; set; }
    public DateTimeOffset? EstimatedInBlockAt { get; set; }
    public string? EstimatedTaxiTime { get; set; }
    public DateTimeOffset? TargetOffBlockAt { get; set; }
    public DateTimeOffset? CalculatedTakeoffAt { get; set; }
    public DateTimeOffset? TargetStartupApprovalAt { get; set; }
    public DateTimeOffset? ActualStartupApprovalAt { get; set; }
    public DateTimeOffset? ActualGroundHandlingAt { get; set; }
    public DateTimeOffset? ActualOffBlockAt { get; set; }
    public DateTimeOffset? ActualInBlockAt { get; set; }
    public DateTimeOffset? ActualRunwayDepartureAt { get; set; }
    public DateTimeOffset? ActualEngineStartAt { get; set; }
    public DateTimeOffset? ActualStartupAt { get; set; }
    public DateTimeOffset? TargetTaxiTime { get; set; }
    public string? ArrivalGate { get; set; }
    public string? DepartureGate { get; set; }
    public DateTimeOffset? EstimatedLandingAt { get; set; }
    public DateTimeOffset? ActualStartRequestAt { get; set; }
    public string? SourceRowIndex { get; set; }
    public string? ArrivalIcaoFlightNumber { get; set; }
    public string? DepartureIcaoFlightNumber { get; set; }
    public DateTimeOffset? EstimatedDepartureAtSource { get; set; }
    public DateTimeOffset? EstimatedArrivalAt { get; set; }
    public string? TargetOffBlockCount { get; set; }
    public string? TargetStartupApprovalCount { get; set; }
}

public class FlightScheduleConfiguration : IEntityTypeConfiguration<FlightSchedule>
{
    public void Configure(EntityTypeBuilder<FlightSchedule> builder)
    {
        builder.HasBaseEntity().UseDefaultTableNaming();
        builder.Property(x => x.FlightNumber).HasColumnType("citext").HasMaxLength(40).IsRequired();
        builder.Property(x => x.FlightDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.Route).HasMaxLength(100).IsRequired();
        builder.Property(x => x.IcaoFlightNumber).HasMaxLength(40);
        builder.Property(x => x.RegistrationNumber).HasColumnType("citext").HasMaxLength(20);
        builder.Property(x => x.AircraftType).HasMaxLength(30);
        builder.Property(x => x.FlightStatus).HasMaxLength(20);
        builder.Property(x => x.ArrivalFlightNumber).HasMaxLength(40);
        builder.Property(x => x.DepartureFlightNumber).HasMaxLength(40);
        builder.Property(x => x.ArrivalStand).HasMaxLength(20);
        builder.Property(x => x.DepartureStand).HasMaxLength(20);
        builder.Property(x => x.Belt).HasMaxLength(20);
        builder.Property(x => x.ArrivalFlightDate).HasColumnType("date");
        builder.Property(x => x.DepartureFlightDate).HasColumnType("date");
        builder.Property(x => x.ArrivalRouteIcao).HasMaxLength(20);
        builder.Property(x => x.DepartureRouteIcao).HasMaxLength(20);
        builder.Property(x => x.HandlingAgent).HasMaxLength(40);
        builder.Property(x => x.LandingRunway).HasMaxLength(20);
        builder.Property(x => x.TakeoffRunway).HasMaxLength(20);
        builder.Property(x => x.NatureOfFlight).HasMaxLength(40);
        builder.Property(x => x.EstimatedTaxiTime).HasMaxLength(30);
        builder.Property(x => x.ArrivalGate).HasMaxLength(20);
        builder.Property(x => x.DepartureGate).HasMaxLength(20);
        builder.Property(x => x.SourceRowIndex).HasMaxLength(30);
        builder.Property(x => x.ArrivalIcaoFlightNumber).HasMaxLength(40);
        builder.Property(x => x.DepartureIcaoFlightNumber).HasMaxLength(40);
        builder.Property(x => x.TargetOffBlockCount).HasMaxLength(30);
        builder.Property(x => x.TargetStartupApprovalCount).HasMaxLength(30);

        builder.HasIndex(x => new { x.FlightNumber, x.FlightDate, x.Route }).IsUnique();
        builder.HasIndex(x => x.FlightDate);
        builder.HasIndex(x => x.RegistrationNumber);
    }
}
