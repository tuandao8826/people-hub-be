using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Roles.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Roles.Queries.Search;

public record SearchRolesQuery(int Page, int PageSize, string? Keyword) : IRequest<PaginationResponse<RoleListItemResponse>>;

public record RoleListItemResponse(Guid Id, string? Code, string Name, string? Description, int PermissionCount, bool IsDefault);

public class SearchRolesHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SearchRolesQuery, PaginationResponse<RoleListItemResponse>>
{
    public Task<PaginationResponse<RoleListItemResponse>> Handle(SearchRolesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Role>().Find().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();
            query = query.Where(role => (role.Code != null && role.Code.ToLower().Contains(keyword))
                || role.Name.ToLower().Contains(keyword));
        }

        var administratorRoleId = RoleManagement.AdministratorRoleId;
        return query.OrderBy(role => role.Name)
            .ThenBy(role => role.Code)
            .Select(role => new RoleListItemResponse(
                role.Id, role.Code, role.Name, role.Description,
                role.RolePermissions!.Count, role.Id == administratorRoleId))
            .ToPageListAsync(new PaginationParams { Page = request.Page, PageSize = request.PageSize }, cancellationToken);
    }
}
