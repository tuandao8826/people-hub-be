using Application.Common.Responses;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

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

    /// <summary>
    /// Chưa làm
    /// - Thêm attribute bỏ qua search
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="query"></param>
    /// <param name="params"></param>
    /// <returns></returns>
    public static IQueryable<TData> ApplySearch<TData>(this IQueryable<TData> query, PaginationParams @params)
    {
        if (string.IsNullOrEmpty(@params.SearchKeyword))
        {
            return query;
        }

        List<string> props = typeof(TData)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(string))
            .Select(x => x.Name)
            .ToList();

        if (props.Count == 0)
        {
            return query;
        }

        string orExpr = string.Join(" || ", props.Select(x => $"(!string.IsNullOrEmpty({x}) && {x}.ToLower().Contains(@0))"));

        return query.Where(orExpr, @params.SearchKeyword.ToLower());
    }

    /// <summary>
    /// Chưa làm
    /// - Check đầu vào
    /// - Convert về đúng định dạng chữ hoa/thường
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="query"></param>
    /// <param name="params"></param>
    /// <returns></returns>
    public static IQueryable<TData> ApplySort<TData>(this IQueryable<TData> query, PaginationParams @params)
    {
        if (@params.Sorts == null || @params.Sorts.Count == 0)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(TData)))
            {
                @params.Sorts = [nameof(BaseEntity.CreatedAt) + $" {PaginationParamSort.DESC}"];
            }
            else
            {
                return query;
            }
        }

        string order = string.Join(", ", @params.Sorts);

        return query.OrderBy(order);
    }
}
