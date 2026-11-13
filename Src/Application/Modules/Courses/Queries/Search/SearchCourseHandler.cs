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
        var query = unitOfWork.Repository<Course>().Find();
        var now = DateTimeOffset.UtcNow;
        if (request.Status.HasValue)
        {
            query = request.Status.Value switch
            {
                CourseStatus.NotStarted => query.Where(course => course.StartDate > now),
                CourseStatus.InProgress => query.Where(course => course.StartDate <= now && course.EndDate.AddDays(1) > now),
                CourseStatus.Completed => query.Where(course => course.EndDate.AddDays(1) <= now),
                CourseStatus.Canceled => query.Where(course => course.Status == CourseStatus.Canceled),
                _ => query
            };
        }

        var result = await query
            .AsNoTracking()
            .ProjectTo<CourseDefaultResponse>(mapper.ConfigurationProvider)
            .ApplySearch(request)
            .ApplySort(request)
            .ToPageListAsync(request, cancellationToken);

        foreach (var course in result.PageItems)
            course.Status = CourseStatusCalculator.Calculate(course.StartDate, course.EndDate, now);

        return result;
    }
}
