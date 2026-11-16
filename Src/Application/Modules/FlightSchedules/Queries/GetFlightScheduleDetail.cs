using Application.Common.Interfaces.Persistence;
using Application.Modules.FlightSchedules.Contracts;
using Application.Modules.FlightSchedules.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FlightSchedules.Queries;

public record GetFlightScheduleDetailQuery(Guid Id) : IRequest<FlightScheduleResponse?>;

public class GetFlightScheduleDetailHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetFlightScheduleDetailQuery, FlightScheduleResponse?>
{
    public Task<FlightScheduleResponse?> Handle(GetFlightScheduleDetailQuery request, CancellationToken cancellationToken) =>
        unitOfWork.Repository<FlightSchedule>().Find(x => x.Id == request.Id).AsNoTracking()
            .Select(x => new FlightScheduleResponse(x.Id, x.FlightNumber, x.IcaoFlightNumber, x.FlightDate, x.Route, x.RegistrationNumber, x.AircraftType, x.FlightStatus, x.ArrivalFlightNumber, x.DepartureFlightNumber, x.ScheduledArrivalAt, x.ScheduledDepartureAt, x.EstimatedDepartureAt, x.ActualLandingAt, x.ActualTakeoffAt, x.ArrivalStand, x.DepartureStand, x.Belt,
                x.ArrivalFlightDate, x.DepartureFlightDate, x.ArrivalRouteIcao, x.DepartureRouteIcao, x.HandlingAgent, x.LandingRunway, x.TakeoffRunway, x.NatureOfFlight,
                x.EstimatedInBlockAt, x.EstimatedTaxiTime, x.TargetOffBlockAt, x.CalculatedTakeoffAt, x.TargetStartupApprovalAt, x.ActualStartupApprovalAt,
                x.ActualGroundHandlingAt, x.ActualOffBlockAt, x.ActualInBlockAt, x.ActualRunwayDepartureAt, x.ActualEngineStartAt, x.ActualStartupAt,
                x.TargetTaxiTime, x.ArrivalGate, x.DepartureGate, x.EstimatedLandingAt, x.ActualStartRequestAt, x.SourceRowIndex,
                x.ArrivalIcaoFlightNumber, x.DepartureIcaoFlightNumber, x.EstimatedDepartureAtSource, x.EstimatedArrivalAt, x.TargetOffBlockCount, x.TargetStartupApprovalCount))
            .FirstOrDefaultAsync(cancellationToken);
}
