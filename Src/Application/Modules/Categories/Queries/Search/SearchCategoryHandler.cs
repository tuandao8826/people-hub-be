using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Categories.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Categories.Queries.Search;

public class SearchCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SearchCategoryQuery, PaginationResponse<CategoryDefaultResponse>>
{
    public async Task<PaginationResponse<CategoryDefaultResponse>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Category>()
            .Find()
            .AsNoTracking()
            .ProjectTo<CategoryDefaultResponse>(mapper.ConfigurationProvider)
            .ToPageListAsync(request, cancellationToken);
    }
}
