using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Bases.Responses;

public class CategoryDefaultResponse : CategoryResponse
{
    /// <summary>
    /// Danh mục cha
    /// </summary>
    public CategoryResponse? Parent { get; set; }
}

public class CategoryDefaultMapping : Profile
{
    public CategoryDefaultMapping()
    {
        CreateMap<Category, CategoryDefaultResponse>();
    }
}

