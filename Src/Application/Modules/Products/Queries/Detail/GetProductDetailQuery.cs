using Application.Modules.Products.Bases.Responses.Products;
using MediatR;

namespace Application.Modules.Products.Queries.Detail;

public record class GetProductDetailQuery(Guid ProductId) : IRequest<ProductDefaultResponse>;
