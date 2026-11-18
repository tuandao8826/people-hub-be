using System.Net;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Requests;
using Application.Modules.Roles.Bases.Responses.Roles;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Queries.Detail;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Commands.Update;

public class UpdateRoleRequest : RoleRequest
{
    public string? Code { get; set; }
}

public record UpdateRoleCommand(Guid Id, UpdateRoleRequest Data) : IRequest<RoleDetailResponse>;

public class UpdateRoleHandler(IUnitOfWork unitOfWork, IMediator mediator)
    : IRequestHandler<UpdateRoleCommand, RoleDetailResponse>
{
    public async Task<RoleDetailResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == RoleManagement.AdministratorRoleId)
            throw new BadRequestException("Không thể sửa vai trò Administrator.", HttpStatusCode.Forbidden);

        var repository = unitOfWork.Repository<Role>();
        var role = await repository.Find(item => item.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException("Không tìm thấy vai trò.", HttpStatusCode.NotFound);

        var codes = await RoleManagement.ValidateAsync(
            unitOfWork, request.Data.Code, request.Data.Name, request.Data.PermissionCodes,
            request.Id, cancellationToken);
        if (request.Data.Description?.Length > 255)
            throw new BadRequestException("Mô tả tối đa 255 ký tự.");

        var rolePermissions = unitOfWork.Repository<RolePermission>();
        var existingCodes = await rolePermissions.Find(item => item.RoleId == role.Id)
            .AsNoTracking()
            .Select(item => item.PermissionCode)
            .ToArrayAsync(cancellationToken);

        role.Code = request.Data.Code!.Trim();
        role.Name = request.Data.Name!.Trim();
        role.Description = request.Data.Description?.Trim();
        var removedCodes = existingCodes.Except(codes, StringComparer.OrdinalIgnoreCase).ToArray();
        var addedCodes = codes.Except(existingCodes, StringComparer.OrdinalIgnoreCase).ToArray();

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await repository.UpdateAsync(role, cancellationToken);
            if (removedCodes.Length > 0)
                await rolePermissions.Find(item => item.RoleId == role.Id && removedCodes.Contains(item.PermissionCode))
                    .ExecuteDeleteAsync(cancellationToken);
            if (addedCodes.Length > 0)
                await rolePermissions.AddRangeAsync(addedCodes.Select(code => new RolePermission(role.Id, code)), cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (RoleManagement.IsDuplicateCode(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new BadRequestException("Mã vai trò đã tồn tại.", HttpStatusCode.Conflict, exception);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return await mediator.Send(new GetRoleDetailQuery(role.Id), cancellationToken);
    }
}
