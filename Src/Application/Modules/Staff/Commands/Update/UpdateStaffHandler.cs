using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Bases.Responses;
using Application.Modules.Staff.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Staff.Commands.Update;

public class UpdateStaffHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateStaffCommand, StaffResponse?>
{
    public async Task<StaffResponse?> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = unitOfWork.Repository<StaffMember>();
        var member = await staff.GetByIdAsync(request.StaffId, cancellationToken);
        if (member is null)
        {
            return null;
        }

        mapper.Map(request.Data, member);

        try
        {
            await staff.UpdateAsync(member, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
        catch (DbUpdateException exception) when (DuplicateStaffCodeException.IsDuplicateCode(exception))
        {
            throw new DuplicateStaffCodeException(exception);
        }

        return mapper.Map<StaffResponse>(member);
    }
}
