using System.Net;
using Application.Common.Auths.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.SystemAccounts.Commands;

public record DeleteSystemAccountCommand(Guid Id) : IRequest<bool>;

public class DeleteSystemAccountHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteSystemAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteSystemAccountCommand request, CancellationToken cancellationToken)
    {
        SystemAccountAccess.EnsureCanDelete(currentUser.Id);

        if (request.Id == SystemAccountAccess.RootAdminId)
            throw new BadRequestException("SystemAccount.RootProtected", HttpStatusCode.Forbidden);

        return await unitOfWork.Repository<User>()
            .Find(user => user.Id == request.Id && user.RoleId == SystemAccountAccess.AdministratorRoleId)
            .ExecuteDeleteAsync(cancellationToken) > 0;
    }
}
