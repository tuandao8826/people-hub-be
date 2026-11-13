using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Staff.Bases.Responses;
using Application.Modules.Staff.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Staff.Queries.Search;

public class SearchStaffHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<SearchStaffQuery, PaginationResponse<StaffResponse>>
{
    public Task<PaginationResponse<StaffResponse>> Handle(SearchStaffQuery request, CancellationToken cancellationToken)
        => unitOfWork.Repository<StaffMember>()
            .Find()
            .AsNoTracking()
            .ApplySearch(request)
            .OrderBy(member => member.Code)
            .ProjectTo<StaffResponse>(mapper.ConfigurationProvider)
            .ToPageListAsync(request, cancellationToken);
}
