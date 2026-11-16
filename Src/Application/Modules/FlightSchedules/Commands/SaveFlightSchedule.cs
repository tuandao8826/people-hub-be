using Application.Common.Interfaces.Persistence;
using Application.Modules.FlightSchedules.Contracts;
using Application.Modules.FlightSchedules.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FlightSchedules.Commands;

public record SaveFlightScheduleCommand(Guid? Id, FlightScheduleRequest Data) : IRequest<Guid>;

public class SaveFlightScheduleHandler(IUnitOfWork unitOfWork) : IRequestHandler<SaveFlightScheduleCommand, Guid>
{
    public async Task<Guid> Handle(SaveFlightScheduleCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        if (string.IsNullOrWhiteSpace(data.FlightNumber) || string.IsNullOrWhiteSpace(data.Route)) throw new ArgumentException("Số hiệu chuyến bay và tuyến bay là bắt buộc.");
        var repository = unitOfWork.Repository<FlightSchedule>();
        var flightNo = data.FlightNumber.Trim().ToUpperInvariant();
        var route = data.Route.Trim().ToUpperInvariant();
        var collision = await repository.Find(x => x.FlightNumber == flightNo && x.FlightDate == data.FlightDate && x.Route == route && x.Id != request.Id).AnyAsync(cancellationToken);
        if (collision) throw new InvalidOperationException($"Chuyến bay {flightNo} ngày {data.FlightDate:dd/MM/yyyy} đã tồn tại.");
        FlightSchedule entity;
        if (request.Id.HasValue)
        {
            entity = await repository.Find(x => x.Id == request.Id.Value).FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException("Không tìm thấy lịch bay.");
        }
        else entity = new FlightSchedule();
        entity.FlightNumber = flightNo; entity.FlightDate = data.FlightDate; entity.Route = route;
        entity.IcaoFlightNumber = Clean(data.IcaoFlightNumber); entity.RegistrationNumber = Clean(data.RegistrationNumber)?.ToUpperInvariant();
        entity.AircraftType = Clean(data.AircraftType); entity.FlightStatus = Clean(data.FlightStatus);
        entity.ArrivalFlightNumber = Clean(data.ArrivalFlightNumber); entity.DepartureFlightNumber = Clean(data.DepartureFlightNumber);
        entity.ScheduledArrivalAt = Utc(data.ScheduledArrivalAt); entity.ScheduledDepartureAt = Utc(data.ScheduledDepartureAt); entity.EstimatedDepartureAt = Utc(data.EstimatedDepartureAt);
        entity.ActualLandingAt = Utc(data.ActualLandingAt); entity.ActualTakeoffAt = Utc(data.ActualTakeoffAt);
        entity.ArrivalStand = Clean(data.ArrivalStand); entity.DepartureStand = Clean(data.DepartureStand); entity.Belt = Clean(data.Belt);
        entity.ArrivalFlightDate=data.ArrivalFlightDate; entity.DepartureFlightDate=data.DepartureFlightDate;
        entity.ArrivalRouteIcao=Clean(data.ArrivalRouteIcao); entity.DepartureRouteIcao=Clean(data.DepartureRouteIcao);
        entity.HandlingAgent=Clean(data.HandlingAgent); entity.LandingRunway=Clean(data.LandingRunway); entity.TakeoffRunway=Clean(data.TakeoffRunway); entity.NatureOfFlight=Clean(data.NatureOfFlight);
        entity.EstimatedInBlockAt=Utc(data.EstimatedInBlockAt); entity.EstimatedTaxiTime=Clean(data.EstimatedTaxiTime); entity.TargetOffBlockAt=Utc(data.TargetOffBlockAt);
        entity.CalculatedTakeoffAt=Utc(data.CalculatedTakeoffAt); entity.TargetStartupApprovalAt=Utc(data.TargetStartupApprovalAt); entity.ActualStartupApprovalAt=Utc(data.ActualStartupApprovalAt);
        entity.ActualGroundHandlingAt=Utc(data.ActualGroundHandlingAt); entity.ActualOffBlockAt=Utc(data.ActualOffBlockAt); entity.ActualInBlockAt=Utc(data.ActualInBlockAt);
        entity.ActualRunwayDepartureAt=Utc(data.ActualRunwayDepartureAt); entity.ActualEngineStartAt=Utc(data.ActualEngineStartAt); entity.ActualStartupAt=Utc(data.ActualStartupAt);
        entity.TargetTaxiTime=Utc(data.TargetTaxiTime); entity.ArrivalGate=Clean(data.ArrivalGate); entity.DepartureGate=Clean(data.DepartureGate);
        entity.EstimatedLandingAt=Utc(data.EstimatedLandingAt); entity.ActualStartRequestAt=Utc(data.ActualStartRequestAt); entity.SourceRowIndex=Clean(data.SourceRowIndex);
        entity.ArrivalIcaoFlightNumber=Clean(data.ArrivalIcaoFlightNumber); entity.DepartureIcaoFlightNumber=Clean(data.DepartureIcaoFlightNumber);
        entity.EstimatedDepartureAtSource=Utc(data.EstimatedDepartureAtSource); entity.EstimatedArrivalAt=Utc(data.EstimatedArrivalAt);
        entity.TargetOffBlockCount=Clean(data.TargetOffBlockCount); entity.TargetStartupApprovalCount=Clean(data.TargetStartupApprovalCount);
        if (request.Id.HasValue) await repository.UpdateAsync(entity, cancellationToken); else await repository.AddAsync(entity, cancellationToken);
        return entity.Id;
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static DateTimeOffset? Utc(DateTimeOffset? value) => value?.ToUniversalTime();
}
