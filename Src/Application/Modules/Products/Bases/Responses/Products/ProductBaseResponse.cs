using Application.Modules.Products.Entities;
using AutoMapper;
using Core.Bases;

namespace Application.Modules.Products.Bases.Responses.Products;

public class ProductBaseResponse : BaseEntity
{
    public string Name { get; set; } = default!;
}

public class ProductBaseMapping : Profile
{
    public ProductBaseMapping()
    {
        CreateMap<Product, ProductBaseResponse>();
    }
}