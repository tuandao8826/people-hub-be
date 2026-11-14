using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.SystemAccounts.Bases;
using Application.Modules.Users.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.SystemAccounts.Queries;

public record SearchSystemAccountsQuery(int Page, int PageSize, string? Keyword = null) : IRequest<PaginationResponse<SystemAccountResponse>>;

public class SearchSystemAccountsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SearchSystemAccountsQuery, PaginationResponse<SystemAccountResponse>>
{
    public Task<PaginationResponse<SystemAccountResponse>> Handle(SearchSystemAccountsQuery request, CancellationToken cancellationToken)
    {
        var rootId = SystemAccountAccess.RootAdminId;
        var roleId = SystemAccountAccess.AdministratorRoleId;
        var query = unitOfWork.Repository<User>().Find(user => user.Id == rootId || user.RoleId == roleId)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();
            query = query.Where(user => user.Username.ToLower().Contains(keyword)
                || user.Name.ToLower().Contains(keyword)
                || (user.Email != null && user.Email.ToLower().Contains(keyword)));
        }

        return query.OrderByDescending(user => user.Id == rootId)
            .ThenBy(user => user.Username)
            .Select(user => new SystemAccountResponse(user.Id, user.Username, user.Name, user.Email, user.Status, user.Id == rootId))
            .ToPageListAsync(new PaginationParams { Page = request.Page, PageSize = request.PageSize }, cancellationToken);
    }
}
