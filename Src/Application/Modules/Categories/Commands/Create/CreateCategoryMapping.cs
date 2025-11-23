using Application.Modules.Categories.Entities;
using AutoMapper;

namespace Application.Modules.Categories.Commands.Create;

public class CreateCategoryMapping : Profile
{
    public CreateCategoryMapping()
    {
        CreateMap<CreateCategoryCommand, Category>();
    }
}
