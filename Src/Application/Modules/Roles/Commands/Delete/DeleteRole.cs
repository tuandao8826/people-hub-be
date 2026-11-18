using System.Net;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Roles.Commands.Delete;

public record DeleteRoleCommand(Guid Id) : IRequest<bool>;

public class DeleteRoleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await RoleManagement.EnsureDeletableAsync(unitOfWork, request.Id, cancellationToken);
        var role = await unitOfWork.Repository<Role>().GetByIdAsync(request.Id, cancellationToken);
        if (role is null) return false;

        try
        {
            await unitOfWork.Repository<Role>().DeleteAsync(role, cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            throw new BadRequestException("Vai trò đang được sử dụng, không thể xóa.", HttpStatusCode.Conflict, exception);
        }
        return true;
    }
}
