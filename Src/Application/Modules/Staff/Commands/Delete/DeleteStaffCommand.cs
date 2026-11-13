using MediatR;

namespace Application.Modules.Staff.Commands.Delete;

public record DeleteStaffCommand(Guid StaffId) : IRequest<bool>;
