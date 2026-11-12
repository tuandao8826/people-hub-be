using Application.Modules.Staff.Bases.Requests;
using Application.Modules.Staff.Bases.Responses;
using MediatR;

namespace Application.Modules.Staff.Commands.Update;

public record UpdateStaffCommand(Guid StaffId, StaffRequest Data) : IRequest<StaffResponse?>;
