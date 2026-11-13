using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Common.Auths.Services;
using Application.Common.Exceptions;
using Application.Modules.SystemAccounts;
using System.Net;
using Application.Modules.Users.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Users.Commands.Delete;

public class DeleteRangeUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteRangeUserCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteRangeUserCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Ids!.ToArray();
        SystemAccountAccess.EnsureCanDelete(currentUser.Id, ids);

        var targets = unitOfWork.Repository<User>().Find(user => ids.Contains(user.Id));
        if (currentUser.Id != SystemAccountAccess.RootAdminId)
        {
            var administratorRoleId = SystemAccountAccess.AdministratorRoleId;
            if (await targets.AnyAsync(user => user.RoleId != administratorRoleId, cancellationToken))
            {
                throw new BadRequestException("SystemAccount.DeleteRequiresRoot", HttpStatusCode.Forbidden);
            }
            targets = targets.Where(user => user.RoleId == administratorRoleId);
        }

        await targets.ExecuteDeleteAsync(cancellationToken);

        return new MultipleIdentiferResponse(ids);
    }
}
