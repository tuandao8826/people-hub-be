using Application.Modules.Products.Bases.Requests.ProductImages;
using Application.Modules.Products.Bases.Requests.Products;
using Application.Modules.Products.Bases.Responses.Products;
using MediatR;

namespace Application.Modules.Products.Commands.Create;

public class CreateProductCommand : ProductRequest, IRequest<ProductDefaultResponse>
{
    public List<ProductImageRequest>? Images { get; set; }
}
