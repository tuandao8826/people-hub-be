using Application.Modules.Roles.Entities;
using AutoMapper;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleMapping : Profile
{
    public CreateRoleMapping()
    {
        CreateMap<CreateRoleCommand, Role>();
    }
}
