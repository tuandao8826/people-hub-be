using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Bases.Responses.Users;
using Application.Modules.Users.Entities;
using Application.Modules.SystemAccounts;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Users.Commands.Update;

public class UpdateUserHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDefaultResponse>
{
    public async Task<UserDefaultResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == SystemAccountAccess.RootAdminId)
        {
            throw new BadRequestException("SystemAccount.RootProtected", HttpStatusCode.Forbidden);
        }

        User user = await unitOfWork.Repository<User>()
            .Find(x => x.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<User>.NotFound());
        
        mapper.Map(request.Data, user);
        user.Password = PasswordExtension.Hash(request.Data.Password!);

        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);
        return mapper.Map<UserDefaultResponse>(user);
    }
}
