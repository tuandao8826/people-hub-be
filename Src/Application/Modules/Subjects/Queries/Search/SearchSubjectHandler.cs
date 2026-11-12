using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Subjects.Bases.Responses;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Subjects.Queries.Search;

public class SearchSubjectHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SearchSubjectQuery, PaginationResponse<SubjectDefaultResponse>>
{
    public async Task<PaginationResponse<SubjectDefaultResponse>> Handle(SearchSubjectQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Subject>()
            .Find()
            .AsNoTracking()
            .ProjectTo<SubjectDefaultResponse>(mapper.ConfigurationProvider)
            .ApplySearch(request)
            .ApplySort(request)
            .ToPageListAsync(request, cancellationToken);
    }
}