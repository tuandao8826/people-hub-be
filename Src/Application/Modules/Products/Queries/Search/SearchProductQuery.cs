using Application.Modules.Products.Bases.Responses.Products;
using MediatR;

namespace Application.Modules.Products.Queries.Search;

public class SearchProductQuery : IRequest<List<ProductDefaultResponse>>;
