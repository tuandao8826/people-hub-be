using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Queries.Detail;
using AutoMapper;
using MediatR;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) : IRequestHandler<CreateRoleCommand, RoleDetailResponse>
{
    public async Task<RoleDetailResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        Role role = mapper.Map<Role>(request);
        role.RolePermissions = request.PermissionCodes!.Select(code => new RolePermission(role.Id, code)).ToArray();

        await unitOfWork.Repository<Role>().AddAsync(role, cancellationToken);

        return await mediator.Send(new GetRoleDetailQuery(role.Id), cancellationToken);
    }
}
