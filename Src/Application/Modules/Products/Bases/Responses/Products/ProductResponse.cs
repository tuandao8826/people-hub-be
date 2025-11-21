using Application.Modules.Products.Entities;
using AutoMapper;

namespace Application.Modules.Products.Bases.Responses.Products;

public class ProductResponse : ProductBaseResponse
{
    public string? Content { get; set; }

    public decimal? Price { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// Ex: Kg, Cái,...
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Ảnh đại diện
    /// </summary>
    public string? Thumbnail { get; set; }
}

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductResponse>();
    }
}