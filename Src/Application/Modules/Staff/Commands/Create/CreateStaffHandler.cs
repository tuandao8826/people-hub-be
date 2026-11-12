using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Bases.Responses;
using Application.Modules.Staff.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Staff.Commands.Create;

public class CreateStaffHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateStaffCommand, StaffResponse>
{
    public async Task<StaffResponse> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var member = mapper.Map<StaffMember>(request);

        try
        {
            await unitOfWork.Repository<StaffMember>().AddAsync(member, cancellationToken);
        }
        catch (DbUpdateException exception) when (DuplicateStaffCodeException.IsDuplicateCode(exception))
        {
            throw new DuplicateStaffCodeException(exception);
        }

        return mapper.Map<StaffResponse>(member);
    }
}
