using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Entities;
using Core.Bases.Interfaces;
using System.Linq.Expressions;

namespace Application.Modules.Roles.Validations;

public static class PermissionValidation
{
    public static bool HasPermissionCode(this IUnitOfWork unitOfWork, string code, Expression<Func<Permission, bool>>? filter = null)
        => unitOfWork.Repository<Permission>().Any(x => x.Code == code);

    public static bool HasPermissionCodes(this IUnitOfWork unitOfWork, IEnumerable<string> codes, Expression<Func<Permission, bool>>? filter = null)
        => unitOfWork.Repository<Permission>().Count(x => codes.Contains(x.Code)) == codes.Count();
}
