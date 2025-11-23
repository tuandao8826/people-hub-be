using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Categories.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Categories.Queries;

public class GetCategoryDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCategoryDetailQuery, CategoryDefaultResponse>
{
    public async Task<CategoryDefaultResponse> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Category>()
            .Find(x => x.Id == request.CategoryId)
            .ProjectTo<CategoryDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<Category>.NotFound());
    }
}
