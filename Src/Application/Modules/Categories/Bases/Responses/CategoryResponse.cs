using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Bases.Responses;

public class CategoryResponse : CategoryBaseResponse
{
}

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryResponse>();
    }
}

