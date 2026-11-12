using Application.Modules.Staff.Entities;

namespace Application.Modules.Staff.Bases.Requests;

public class StaffRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Department { get; set; }

    public string? JobTitle { get; set; }

    public StaffStatus Status { get; set; } = StaffStatus.Active;

    public DateTimeOffset? JoinedOn { get; set; }
}
