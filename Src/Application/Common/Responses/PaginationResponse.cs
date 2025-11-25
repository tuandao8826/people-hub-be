using Core.Bases;
using FluentValidation;

namespace Application.Common.Responses;

public record class PaginationResponse<TData>(IReadOnlyList<TData> PageItems, PageInfo PageInfo);

public record class PageInfo(int Page = 1, int PageSize = 10, int ItemCount = 0, int TotalCount = 0)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    public bool HasPrevious => Page > 1;
    
    public bool HasNext => Page < TotalPages;
}

public class PaginationParams
{
    public int Page { get; init; } = 1;

    public int PageSize { get; set; } = 10;

    public List<string> Sorts { get; set; } = [];

    public string? SearchKeyword { get; set; }
}

public enum PaginationParamSort
{
    ASC,
    DESC,
}