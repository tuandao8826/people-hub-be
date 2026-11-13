using Application.Common.Auths.Services;
using Application.Common.Interfaces.Persistence;
using Application.Modules.SystemAccounts;
using Application.Modules.Users.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Api.Common.Auths;

internal sealed class ManageSystemAccountsRequirement : IAuthorizationRequirement;

internal sealed class SystemAccountAuthorization(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : AuthorizationHandler<ManageSystemAccountsRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ManageSystemAccountsRequirement requirement)
    {
        if (currentUser.Id == Guid.Empty) return;

        var user = await unitOfWork.Repository<User>().GetByIdAsync(currentUser.Id);
        if (SystemAccountAccess.CanManage(user)) context.Succeed(requirement);
    }
}
