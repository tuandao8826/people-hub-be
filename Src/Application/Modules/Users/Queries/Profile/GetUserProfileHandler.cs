using Application.Common.Auths.Services;
using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Bases.Responses.Users;
using Application.Modules.Users.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Users.Queries.Profile;

public class GetUserProfileHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetUserProfileQuery, UserDefaultResponse>
{
    public async Task<UserDefaultResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<User>()
            .Find(x => x.Id == currentUserService.Id)
            .AsNoTracking()
            .ProjectTo<UserDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<User>.NotFound());
    }
}
