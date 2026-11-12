using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Courses.Bases.Responses.Courses;
using Application.Modules.Courses.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Queries.Detail;

public class GetCourseDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCourseDetailQuery, CourseDefaultResponse>
{
    public async Task<CourseDefaultResponse> Handle(GetCourseDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Course>()
            .Find(x => x.Id == request.CourseId)
            .AsNoTracking()
            .ProjectTo<CourseDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<Course>.NotFound());
    }
}
