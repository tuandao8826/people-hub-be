using Application.Modules.Staff.Bases.Responses;
using MediatR;

namespace Application.Modules.Staff.Queries.Detail;

public record GetStaffDetailQuery(Guid StaffId) : IRequest<StaffResponse?>;
