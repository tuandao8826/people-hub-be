using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Responses.Permissions;
using Application.Modules.Roles.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Queries.Permissions;

public record GetRolePermissionsQuery : IRequest<IReadOnlyList<PermissionBaseResponse>>;

public class GetRolePermissionsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetRolePermissionsQuery, IReadOnlyList<PermissionBaseResponse>>
{
    public async Task<IReadOnlyList<PermissionBaseResponse>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Repository<Permission>().Find().AsNoTracking()
            .OrderBy(permission => permission.Name)
            .ProjectTo<PermissionBaseResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
}
