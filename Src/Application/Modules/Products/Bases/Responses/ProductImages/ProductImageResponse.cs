using Application.Modules.Products.Entities;
using AutoMapper;
using Core.Bases;

namespace Application.Modules.Products.Bases.Responses.ProductImages;

public class ProductImageResponse : BaseEntity
{
    public string Path { get; set; } = default!;

    public int Order { get; set; }
}

public class ProductImageMapping : Profile
{
    public ProductImageMapping()
    {
        CreateMap<ProductImage, ProductImageResponse>();
    }
}
