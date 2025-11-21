using Application.Common.Interfaces.Persistence;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Products.Queries.Search;

public class SearchProductHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SearchProductQuery, List<ProductDefaultResponse>>
{
    public async Task<List<ProductDefaultResponse>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Product>()
            .Find()
            .AsNoTracking()
            .ProjectTo<ProductDefaultResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
