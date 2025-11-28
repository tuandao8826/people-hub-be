using Application.Common.Responses;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

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
    /// <typeparam name="TSource"></typeparam>
    /// <param name="query"></param>
    /// <param name="params"></param>
    /// <returns></returns>
    public static IQueryable<TSource> ApplySearch<TSource>(this IQueryable<TSource> query, PaginationParams @params)
    {
        if (string.IsNullOrEmpty(@params.Search?.Keyword))
        {
            return query;
        }

        PropertyInfo[] props = typeof(TSource)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(string))
            .ToArray();

        if (props.Length == 0)
        {
            return query;
        }

        string keywordValue = @params.Search.IgnoreCase ? @params.Search.Keyword.ToLower() : @params.Search.Keyword;

        ParameterExpression param = Expression.Parameter(typeof(TSource), "x");
        Expression? body = null;
        Expression keyword = Expression.Constant(keywordValue);
        MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        MethodInfo containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;

        foreach (PropertyInfo prop in props)
        {
            MemberExpression member = Expression.Property(param, prop);

            Expression notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
            Expression toLowerMember = @params.Search.IgnoreCase ? Expression.Call(member, toLowerMethod) : member;
            Expression contains = Expression.Call(toLowerMember, containsMethod, keyword);

            Expression condition = Expression.AndAlso(notNull, contains);

            body = body is null ? condition : Expression.OrElse(body, condition);
        }

        Expression<Func<TSource, bool>> predicate = Expression.Lambda<Func<TSource, bool>>(body ?? Expression.Constant(true), param);

        return query.Where(predicate);
    }

    /// <summary>
    /// Chưa làm
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="query"></param>
    /// <param name="params"></param>
    /// <returns></returns>
    public static IQueryable<TSource> ApplySort<TSource>(this IQueryable<TSource> query, PaginationParams @params)
    {
        if (@params.Sorts == null || @params.Sorts.Count == 0)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(TSource)))
            {
                @params.Sorts = [ new PaginationParamSort { Field = nameof(BaseEntity.CreatedAt), Direction = SortDirection.DESC }];
            }
            else
            {
                return query;
            }
        }

        PropertyInfo[] props = typeof(TSource)
            .GetProperties()
            .Where(x => @params.Sorts.Any(s => s.Field.ToLower().Equals(x.Name.ToLower())))
            .ToArray();

        if (props.Length == 0)
        {
            return query;
        }

        List<PaginationParamSort> sorts = [];

        foreach (PropertyInfo prop in props)
        {
            PaginationParamSort? sort = @params.Sorts.Where(x => x.Field.Contains(prop.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (sort is null)
            {
                continue;
            }

            sort.Field = prop.Name;
            sorts.Add(sort);
        }

        string order = string.Join(", ", sorts.Select(x => $"{x.Field} {x.Direction}"));

        return query.OrderBy(order);
    }
}
