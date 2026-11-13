using System.ComponentModel.DataAnnotations;
using Core.Common.Enums;

namespace Application.Modules.SystemAccounts.Bases;

public record SystemAccountResponse(Guid Id, string Username, string Name, string? Email, OperationStatus Status, bool IsRootAdmin);

public class UpdateSystemAccountRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [StringLength(50, MinimumLength = 6)]
    public string? Password { get; set; }

    [EnumDataType(typeof(OperationStatus))]
    public OperationStatus Status { get; set; } = OperationStatus.Active;
}

public class CreateSystemAccountRequest : UpdateSystemAccountRequest
{
    [Required, StringLength(50, MinimumLength = 6)]
    public string Username { get; set; } = null!;
}
