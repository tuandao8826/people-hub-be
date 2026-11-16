using Application.Common.Responses;
using Application.Modules.FlightSchedules.Contracts;
using MediatR;

namespace Application.Modules.FlightSchedules.Queries;

public class SearchFlightSchedulesQuery : PaginationParams, IRequest<PaginationResponse<FlightScheduleListItemResponse>>
{
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public string? FlightNumber { get; init; }
    public string? RegistrationNumber { get; init; }
    public string? Stand { get; init; }
}
