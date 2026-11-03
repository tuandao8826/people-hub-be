using Application.Modules.Products.Entities;
using AutoMapper;

namespace Application.Modules.Products.Commands.Update;

internal class UpdateProductMapping : Profile
{
    public UpdateProductMapping()
    {
        CreateMap<UpdateProductRequest, Product>();
    }
}
