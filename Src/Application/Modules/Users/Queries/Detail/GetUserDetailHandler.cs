using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Bases.Responses.Users;
using Application.Modules.Users.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Users.Queries.Detail;

public class GetUserDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetUserDetailQuery, UserDefaultResponse>
{
    public async Task<UserDefaultResponse> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<User>()
            .Find(x => x.Id == request.UserId)
            .AsNoTracking()
            .ProjectTo<UserDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<User>.NotFound());
    }
}
