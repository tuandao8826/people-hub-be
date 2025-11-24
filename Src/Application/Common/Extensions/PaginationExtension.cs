using Application.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class PaginationExtension
{
    public static async Task<PaginationResponse<TData>> ToPageListAsync<TData>(this IQueryable<TData> query, PaginationParams @params, CancellationToken cancellationToken = default)
    {
        int totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .Skip((@params.Page - 1) * @params.PageSize)
            .Take(@params.PageSize)
            .ToListAsync(cancellationToken);

        return new(result, new(@params.Page, @params.PageSize, result.Count, totalCount));
    }
}
