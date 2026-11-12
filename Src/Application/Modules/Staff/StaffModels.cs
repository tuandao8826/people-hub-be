using Application.Modules.Staff.Entities;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Staff;

public class StaffRequest
{
    [Required, MaxLength(30)]
    public string Code { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [EmailAddress, MaxLength(254)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    [EnumDataType(typeof(StaffStatus))]
    public StaffStatus Status { get; set; } = StaffStatus.Active;

    public DateTimeOffset? JoinedOn { get; set; }
}

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

public class StaffMapping : Profile
{
    public StaffMapping()
    {
        CreateMap<StaffMember, StaffResponse>();
    }
}
