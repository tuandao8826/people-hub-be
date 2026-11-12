using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Courses.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Commands.Delete;

public class DeleteCourseHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCourseCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.Repository<Course>()
            .Find(x => request.Ids!.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return new MultipleIdentiferResponse(request.Ids!);
    }
}