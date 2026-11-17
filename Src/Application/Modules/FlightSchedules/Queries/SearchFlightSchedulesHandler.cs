using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.FlightSchedules.Contracts;
using Application.Modules.FlightSchedules.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FlightSchedules.Queries;

public class SearchFlightSchedulesHandler(IUnitOfWork unitOfWork) : IRequestHandler<SearchFlightSchedulesQuery, PaginationResponse<FlightScheduleListItemResponse>>
{
    public async Task<PaginationResponse<FlightScheduleListItemResponse>> Handle(SearchFlightSchedulesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<FlightSchedule>().Find().AsNoTracking();
        if (request.DateFrom.HasValue) query = query.Where(x => x.FlightDate >= request.DateFrom);
        if (request.DateTo.HasValue) query = query.Where(x => x.FlightDate <= request.DateTo);
        var flight = request.FlightNumber?.Trim();
        if (!string.IsNullOrEmpty(flight)) query = query.Where(x => EF.Functions.ILike(x.FlightNumber, $"%{flight}%") || (x.IcaoFlightNumber != null && EF.Functions.ILike(x.IcaoFlightNumber, $"%{flight}%")) || (x.ArrivalFlightNumber != null && EF.Functions.ILike(x.ArrivalFlightNumber, $"%{flight}%")) || (x.DepartureFlightNumber != null && EF.Functions.ILike(x.DepartureFlightNumber, $"%{flight}%")));
        var registration = request.RegistrationNumber?.Trim();
        if (!string.IsNullOrEmpty(registration)) query = query.Where(x => x.RegistrationNumber != null && EF.Functions.ILike(x.RegistrationNumber, $"%{registration}%"));
        var stand = request.Stand?.Trim();
        if (!string.IsNullOrEmpty(stand)) query = query.Where(x => (x.ArrivalStand != null && EF.Functions.ILike(x.ArrivalStand, $"%{stand}%")) || (x.DepartureStand != null && EF.Functions.ILike(x.DepartureStand, $"%{stand}%")));
        var total = await query.CountAsync(cancellationToken);
        var page = Math.Max(1, request.Page);
        var size = Math.Clamp(request.PageSize, 1, 100);
        var items = await query.OrderByDescending(x => x.ScheduledArrivalAt).ThenByDescending(x => x.FlightDate).ThenBy(x => x.FlightNumber)
            .Skip((page - 1) * size).Take(size)
            .Select(x => new FlightScheduleListItemResponse(x.Id, x.FlightNumber, x.ArrivalFlightNumber, x.DepartureFlightNumber,
                x.FlightDate, x.ArrivalFlightDate, x.DepartureFlightDate, x.Route, x.RegistrationNumber, x.AircraftType,
                x.HandlingAgent, x.ArrivalStand, x.DepartureStand, x.Belt, x.ArrivalGate, x.DepartureGate,
                x.ScheduledArrivalAt, x.ActualInBlockAt, x.ScheduledDepartureAt))
            .ToListAsync(cancellationToken);
        return new PaginationResponse<FlightScheduleListItemResponse>(items, new PageInfo(page, size, items.Count, total));
    }
}
