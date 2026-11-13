using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Users.Entities;
using Application.Modules.SystemAccounts;
using AutoMapper;

namespace Application.Modules.Users.Bases.Responses.Users;

public class UserDefaultResponse : UserBaseResponse
{
    public RoleBaseResponse? Role { get; set; }

    public bool CanManageSystemAccounts => Status == Core.Common.Enums.OperationStatus.Active
        && (Id == SystemAccountAccess.RootAdminId || Role?.Id == SystemAccountAccess.AdministratorRoleId);

    public bool CanDeleteSystemAccounts => CanManageSystemAccounts;
}

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDefaultResponse>();
    }
}
