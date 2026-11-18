using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Queries.Options;

public record RoleOptionResponse(Guid Id, string? Code, string Name);

public record GetRoleOptionsQuery : IRequest<IReadOnlyList<RoleOptionResponse>>;

public class GetRoleOptionsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRoleOptionsQuery, IReadOnlyList<RoleOptionResponse>>
{
    public async Task<IReadOnlyList<RoleOptionResponse>> Handle(GetRoleOptionsQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Repository<Role>().Find().AsNoTracking()
            .OrderBy(role => role.Name)
            .ThenBy(role => role.Code)
            .Select(role => new RoleOptionResponse(role.Id, role.Code, role.Name))
            .ToListAsync(cancellationToken);
}
