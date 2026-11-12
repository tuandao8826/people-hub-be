using Application.Modules.Staff.Entities;
using AutoMapper;

namespace Application.Modules.Staff.Bases.Responses;

public record StaffResponse(
    Guid Id,
    string Code,
    string Name,
    string? Email,
    string? Phone,
    string? Department,
    string? JobTitle,
    StaffStatus Status,
    DateTimeOffset? JoinedOn);

public class StaffResponseMapping : Profile
{
    public StaffResponseMapping()
    {
        CreateMap<StaffMember, StaffResponse>();
    }
}
