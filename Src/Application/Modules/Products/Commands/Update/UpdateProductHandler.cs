using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Products.Commands.Update;

internal class UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateProductCommand, ProductDefaultResponse>
{
    public async Task<ProductDefaultResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new BadRequestException(nameof(request));

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            await unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new BadRequestException(Message<Product>.Create(false), innerException: ex);
        }

        return await unitOfWork.Repository<Product>()
            .Find(x => x.Id == product.Id)
            .ProjectTo<ProductDefaultResponse>(mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}
