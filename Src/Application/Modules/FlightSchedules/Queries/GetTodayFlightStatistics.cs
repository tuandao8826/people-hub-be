using Application.Common.Interfaces.Persistence;
using Application.Modules.FlightSchedules.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FlightSchedules.Queries;

public sealed record GetFlightStatisticsQuery(DateOnly? DateFrom, DateOnly? DateTo) : IRequest<FlightStatisticsResponse>;

public sealed record FlightStatisticsResponse(DateOnly? DateFrom, DateOnly? DateTo, int TotalFlights, int ArrivalFlights, int DepartureFlights);

public sealed class GetFlightStatisticsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetFlightStatisticsQuery, FlightStatisticsResponse>
{
    public async Task<FlightStatisticsResponse> Handle(GetFlightStatisticsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
        var dateFrom = request.DateFrom ?? (request.DateTo is null ? today : null);
        var dateTo = request.DateTo ?? (request.DateFrom is null ? today : null);
        var flights = unitOfWork.Repository<FlightSchedule>().Find().AsNoTracking();
        var result = await flights.GroupBy(_ => 1).Select(group => new
        {
            Total = group.Count(x =>
                (x.ArrivalFlightNumber != null && (!dateFrom.HasValue || (x.ArrivalFlightDate ?? x.FlightDate) >= dateFrom.Value) && (!dateTo.HasValue || (x.ArrivalFlightDate ?? x.FlightDate) <= dateTo.Value)) ||
                (x.DepartureFlightNumber != null && (!dateFrom.HasValue || (x.DepartureFlightDate ?? x.FlightDate) >= dateFrom.Value) && (!dateTo.HasValue || (x.DepartureFlightDate ?? x.FlightDate) <= dateTo.Value)) ||
                (x.ArrivalFlightNumber == null && x.DepartureFlightNumber == null && (!dateFrom.HasValue || x.FlightDate >= dateFrom.Value) && (!dateTo.HasValue || x.FlightDate <= dateTo.Value))),
            Arrivals = group.Count(x => x.ArrivalFlightNumber != null && (!dateFrom.HasValue || (x.ArrivalFlightDate ?? x.FlightDate) >= dateFrom.Value) && (!dateTo.HasValue || (x.ArrivalFlightDate ?? x.FlightDate) <= dateTo.Value)),
            Departures = group.Count(x => x.DepartureFlightNumber != null && (!dateFrom.HasValue || (x.DepartureFlightDate ?? x.FlightDate) >= dateFrom.Value) && (!dateTo.HasValue || (x.DepartureFlightDate ?? x.FlightDate) <= dateTo.Value))
        }).FirstOrDefaultAsync(cancellationToken);

        return new FlightStatisticsResponse(dateFrom, dateTo, result?.Total ?? 0, result?.Arrivals ?? 0, result?.Departures ?? 0);
    }
}
