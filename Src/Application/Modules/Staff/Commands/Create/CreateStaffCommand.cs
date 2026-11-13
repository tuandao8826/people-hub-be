using Application.Modules.Staff.Bases.Requests;
using Application.Modules.Staff.Bases.Responses;
using MediatR;

namespace Application.Modules.Staff.Commands.Create;

public class CreateStaffCommand : StaffRequest, IRequest<StaffResponse>;
