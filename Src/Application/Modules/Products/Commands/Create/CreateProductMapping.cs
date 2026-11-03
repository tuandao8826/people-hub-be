using Application.Modules.Products.Entities;
using AutoMapper;

namespace Application.Modules.Products.Commands.Create;
public class CreateProductMapping : Profile
{
    public CreateProductMapping()
    {
        CreateMap<CreateProductCommand, Product>();
    }
}
