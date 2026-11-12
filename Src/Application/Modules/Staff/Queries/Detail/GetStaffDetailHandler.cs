using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Bases.Responses;
using Application.Modules.Staff.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Staff.Queries.Detail;

public class GetStaffDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetStaffDetailQuery, StaffResponse?>
{
    public Task<StaffResponse?> Handle(GetStaffDetailQuery request, CancellationToken cancellationToken)
        => unitOfWork.Repository<StaffMember>()
            .Find(member => member.Id == request.StaffId)
            .AsNoTracking()
            .ProjectTo<StaffResponse>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken);
}
