using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Staff.Commands.Delete;

public class DeleteStaffHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteStaffCommand, bool>
{
    public async Task<bool> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
        => await unitOfWork.Repository<StaffMember>()
            .Find(member => member.Id == request.StaffId)
            .ExecuteDeleteAsync(cancellationToken) > 0;
}
