using Application.Modules.Roles.Bases.Requests;
using Application.Modules.Roles.Bases.Responses.Roles;
using MediatR;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleCommand : RoleRequest, IRequest<RoleDetailResponse>
{
    public string? Code { get; set; }
}