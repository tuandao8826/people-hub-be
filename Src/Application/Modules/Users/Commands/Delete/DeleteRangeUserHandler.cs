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
        SystemAccountAccess.EnsureCanDelete(currentUser.Id);

        if (request.Ids!.Contains(SystemAccountAccess.RootAdminId))
        {
            throw new BadRequestException("SystemAccount.RootProtected", HttpStatusCode.Forbidden);
        }

        await unitOfWork.Repository<User>().Find(x => request.Ids!.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);

        return new MultipleIdentiferResponse(request.Ids!);
    }
}
