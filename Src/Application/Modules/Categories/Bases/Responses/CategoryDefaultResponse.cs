using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Bases.Responses;

public class CategoryDefaultResponse : CategoryResponse
{
}

public class CategoryDefaultMapping : Profile
{
    public CategoryDefaultMapping()
    {
        CreateMap<Category, CategoryDefaultResponse>();
    }
}

