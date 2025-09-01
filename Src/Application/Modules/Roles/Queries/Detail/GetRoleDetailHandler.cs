using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Queries.Detail;

public class GetRoleDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetRoleDetailQuery, RoleDetailResponse>
{
    public async Task<RoleDetailResponse> Handle(GetRoleDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Role>()
            .Find(x => x.Id == request.RoleId)
            .AsNoTracking()
            .ProjectTo<RoleDetailResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<Role>.NotFound());
    }
}
