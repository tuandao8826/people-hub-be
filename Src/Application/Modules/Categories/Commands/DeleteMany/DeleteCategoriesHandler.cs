using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Categories.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Categories.Commands.DeleteMany;

public class DeleteCategoriesHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoriesCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteCategoriesCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.Repository<Category>().Find(x => request.Ids!.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);

        return new MultipleIdentiferResponse(request.Ids!);
    }
}
