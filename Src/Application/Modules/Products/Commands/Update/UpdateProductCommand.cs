using Application.Modules.Products.Bases.Requests.Products;
using Application.Modules.Products.Bases.Responses.Products;
using MediatR;

namespace Application.Modules.Products.Commands.Update;

public record class UpdateProductCommand(Guid ProductId, UpdateProductRequest Data) : IRequest<ProductDefaultResponse>;

public class UpdateProductRequest : ProductRequest
{

}