using Application.Modules.Categories.Entities;
using AutoMapper;
using Core.Bases;

namespace Application.Modules.Categories.Bases.Responses;

public class CategoryBaseResponse : BaseEntity
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}

public class CategoryBaseMapping : Profile
{
    public CategoryBaseMapping()
    {
        CreateMap<Category, CategoryBaseResponse>();
    }
}
