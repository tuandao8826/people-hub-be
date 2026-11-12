using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Subjects.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Subjects.Commands.Delete;

public class DeleteSubjectHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSubjectCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        // Thực hiện xóa trực tiếp các bản ghi có ID nằm trong danh sách request.Ids
        await unitOfWork.Repository<Subject>()
            .Find(x => request.Ids!.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return new MultipleIdentiferResponse(request.Ids!);
    }
}