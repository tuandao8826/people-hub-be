using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Products.Queries.Detail;

public class GetProductDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetProductDetailQuery, ProductDefaultResponse>
{
    public async Task<ProductDefaultResponse> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Product>()
            .Find(x => x.Id == request.ProductId)
            .AsQueryable()
            .ProjectTo<ProductDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<Product>.NotFound());
    }
}
