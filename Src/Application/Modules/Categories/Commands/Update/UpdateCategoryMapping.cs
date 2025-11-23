using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Commands.Update;

public class UpdateCategoryMapping : Profile
{
    public UpdateCategoryMapping()
    {
        CreateMap<UpdateCategoryRequest, Category>();
    }
}
