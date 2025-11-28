using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Products.Queries.Search;

public class SearchProductHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SearchProductQuery, PaginationResponse<ProductDefaultResponse>>
{
    public async Task<PaginationResponse<ProductDefaultResponse>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Product>()
            .Find()
            .AsNoTracking()
            .ProjectTo<ProductDefaultResponse>(mapper.ConfigurationProvider)
            .ApplySearch(request)
            .ApplySort(request)
            .ToPageListAsync(request, cancellationToken);
    }
}
