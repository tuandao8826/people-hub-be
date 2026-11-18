using System.Net;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Queries.Detail;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) : IRequestHandler<CreateRoleCommand, RoleDetailResponse>
{
    public async Task<RoleDetailResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var permissionCodes = await RoleManagement.ValidateAsync(unitOfWork, request.Code, request.Name, request.PermissionCodes, null, cancellationToken);
        if (request.Description?.Length > 255)
            throw new BadRequestException("Mô tả tối đa 255 ký tự.");

        request.Code = request.Code!.Trim();
        request.Name = request.Name!.Trim();
        request.Description = request.Description?.Trim();
        var role = mapper.Map<Role>(request);
        role.RolePermissions = permissionCodes.Select(code => new RolePermission(role.Id, code)).ToArray();

        try
        {
            await unitOfWork.Repository<Role>().AddAsync(role, cancellationToken);
        }
        catch (DbUpdateException exception) when (RoleManagement.IsDuplicateCode(exception))
        {
            throw new BadRequestException("Mã vai trò đã tồn tại.", HttpStatusCode.Conflict, exception);
        }

        return await mediator.Send(new GetRoleDetailQuery(role.Id), cancellationToken);
    }
}
