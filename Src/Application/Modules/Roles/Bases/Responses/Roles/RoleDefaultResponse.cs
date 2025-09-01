using Application.Modules.Roles.Entities;
using AutoMapper;

namespace Application.Modules.Roles.Bases.Responses.Roles;

public class RoleDefaultResponse : RoleBaseResponse;

public class RoleDefaultMapping : Profile
{
    public RoleDefaultMapping()
    {
        CreateMap<Role, RoleDefaultResponse>();
    }
}
