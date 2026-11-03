using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Products.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Products.Commands.DeleteMany;

public class DeleteProductsHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductsCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteProductsCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.Repository<Product>().Find(x => request.Ids!.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
        return new(request.Ids!);
    }
}
