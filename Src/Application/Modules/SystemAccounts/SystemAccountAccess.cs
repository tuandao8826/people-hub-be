using System.Net;
using Application.Common.Definitions.Constants;
using Application.Common.Exceptions;
using Application.Modules.Users.Entities;
using Core.Common.Enums;

namespace Application.Modules.SystemAccounts;

public static class SystemAccountAccess
{
    public const string Policy = "ManageSystemAccounts";
    public static readonly Guid RootAdminId = Guid.Parse(AdminEntityConstants.Id);
    public static readonly Guid AdministratorRoleId = Guid.Parse(RoleEntityConstants.Id);

    public static bool CanManage(User? user)
        => user?.Status == OperationStatus.Active
            && (user.Id == RootAdminId || user.RoleId == AdministratorRoleId);

    public static void EnsureCanDelete(Guid actorId, IEnumerable<Guid> targetIds)
    {
        if (targetIds.Contains(RootAdminId))
        {
            throw new BadRequestException("SystemAccount.RootProtected", HttpStatusCode.Forbidden);
        }

        if (targetIds.Contains(actorId))
        {
            throw new BadRequestException("SystemAccount.SelfDeleteForbidden", HttpStatusCode.Forbidden);
        }
    }
}
