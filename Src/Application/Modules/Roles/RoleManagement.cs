using System.Net;
using Application.Common.Definitions.Constants;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Entities;
using Application.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Roles;

internal static class RoleManagement
{
    public static readonly Guid AdministratorRoleId = Guid.Parse(RoleEntityConstants.Id);

    public static async Task<string[]> ValidateAsync(
        IUnitOfWork unitOfWork, string? code, string? name, IEnumerable<string>? permissionCodes,
        Guid? existingId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 255)
            throw new BadRequestException("Mã vai trò là bắt buộc và tối đa 255 ký tự.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 255)
            throw new BadRequestException("Tên vai trò là bắt buộc và tối đa 255 ký tự.");

        var normalizedCode = code.Trim().ToLower();
        var roles = unitOfWork.Repository<Role>();
        var codeExists = existingId is null
            ? await roles.AnyAsync(role => role.Code != null && role.Code.ToLower() == normalizedCode, cancellationToken)
            : await roles.AnyAsync(role => role.Id != existingId.Value && role.Code != null && role.Code.ToLower() == normalizedCode, cancellationToken);
        if (codeExists)
            throw new BadRequestException("Mã vai trò đã tồn tại.", HttpStatusCode.Conflict);

        var codes = permissionCodes?.Select(value => value?.Trim() ?? "").ToArray() ?? [];
        if (codes.Length == 0 || codes.Any(string.IsNullOrWhiteSpace))
            throw new BadRequestException("Chọn ít nhất một quyền cho vai trò.");
        if (codes.Distinct(StringComparer.OrdinalIgnoreCase).Count() != codes.Length)
            throw new BadRequestException("Danh sách quyền bị trùng.");

        var availableCodes = await unitOfWork.Repository<Permission>()
            .Find(permission => codes.Contains(permission.Code!))
            .Select(permission => permission.Code!)
            .ToArrayAsync(cancellationToken);
        if (availableCodes.Length != codes.Length)
            throw new BadRequestException("Có quyền không tồn tại trong hệ thống.");

        return availableCodes;
    }

    public static async Task EnsureDeletableAsync(IUnitOfWork unitOfWork, Guid id, CancellationToken cancellationToken)
    {
        if (id == AdministratorRoleId)
            throw new BadRequestException("Không thể xóa vai trò quản trị mặc định.", HttpStatusCode.Forbidden);
        if (await unitOfWork.Repository<User>().AnyAsync(user => user.RoleId == id, cancellationToken))
            throw new BadRequestException("Vai trò đang được gán cho tài khoản, không thể xóa.", HttpStatusCode.Conflict);
    }

    public static bool IsDuplicateCode(DbUpdateException exception)
        => exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}
