using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.Courses;
using Application.Modules.Courses.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Queries.Search;

public class SearchCourseHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<SearchCourseQuery, PaginationResponse<CourseDefaultResponse>>
{
    public async Task<PaginationResponse<CourseDefaultResponse>> Handle(SearchCourseQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Course>()
            .Find()
            .AsNoTracking()
            .ProjectTo<CourseDefaultResponse>(mapper.ConfigurationProvider)
            .ApplySearch(request)
            .ApplySort(request)
            .ToPageListAsync(request, cancellationToken);
    }
}
