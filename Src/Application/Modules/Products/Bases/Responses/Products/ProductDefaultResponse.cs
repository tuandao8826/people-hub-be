using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Products.Bases.Responses.ProductImages;
using Application.Modules.Products.Entities;
using AutoMapper;

namespace Application.Modules.Products.Bases.Responses.Products;

public class ProductDefaultResponse : ProductResponse
{
    public CategoryDefaultResponse? Category { get; set; }

    public ICollection<ProductImageResponse>? Images { get; set; }
}

public class ProductDefaultMapping : Profile
{
    public ProductDefaultMapping()
    {
        CreateMap<Product, ProductDefaultResponse>();
    }
}