using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Bases.Responses;

public class CategoryResponse : CategoryBaseResponse
{
    /// <summary>
    /// Danh mục cha
    /// </summary>
    public CategoryResponse? Parent { get; set; }
}

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryResponse>();
    }
}

