using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Courses.Bases.Responses.CourseParticipant;
using Application.Modules.Courses.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Queries.Participants;

public class SearchCourseParticipantsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<SearchCourseParticipantsQuery, PaginationResponse<CourseParticipantResponse>>
{
    public async Task<PaginationResponse<CourseParticipantResponse>> Handle(
        SearchCourseParticipantsQuery request,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<CourseParticipant>()
            .Find(participant => participant.CourseId == request.CourseId)
            .AsNoTracking()
            .ProjectTo<CourseParticipantResponse>(mapper.ConfigurationProvider)
            .ApplySearch(request)
            .ApplySort(request)
            .ToPageListAsync(request, cancellationToken);
    }
}
