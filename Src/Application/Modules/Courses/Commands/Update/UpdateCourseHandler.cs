using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Courses.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Commands.Update;

public class UpdateCourseHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCourseCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>()
            .Find(x => x.Id == request.CourseId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException($"Không tìm thấy khóa học với ID: {request.CourseId}");

        if (course.CourseCode != request.Data.CourseCode)
        {
            var codeExists = await unitOfWork.Repository<Course>()
                .AnyAsync(x => x.CourseCode == request.Data.CourseCode && x.Id != request.CourseId, cancellationToken);

            if (codeExists)
            {
                throw new BadRequestException($"Mã khóa học '{request.Data.CourseCode}' đã tồn tại trong hệ thống.");
            }
        }

        mapper.Map(request.Data, course);
        course.Status = CourseStatusCalculator.Calculate(course.StartDate, course.EndDate, DateTimeOffset.UtcNow);
        await unitOfWork.Repository<Course>().UpdateAsync(course, cancellationToken);

        if (request.Data.ReplaceStaffMemberList)
        {
            var requestedStaffIds = (request.Data.StaffMemberIds ?? []).Distinct().ToHashSet();
            var participantRepository = unitOfWork.Repository<CourseParticipant>();
            var currentParticipants = await participantRepository
                .Find(participant => participant.CourseId == request.CourseId)
                .ToListAsync(cancellationToken);
            var participantsToRemove = currentParticipants
                .Where(participant => !requestedStaffIds.Contains(participant.StaffMemberId))
                .ToList();
            var currentStaffIds = currentParticipants
                .Select(participant => participant.StaffMemberId)
                .ToHashSet();
            var participantsToAdd = requestedStaffIds
                .Where(staffId => !currentStaffIds.Contains(staffId))
                .Select(staffId => new CourseParticipant
                {
                    CourseId = request.CourseId,
                    StaffMemberId = staffId,
                    IsCompleted = false
                })
                .ToList();

            if (participantsToRemove.Count > 0)
                await participantRepository.DeleteRangeAsync(participantsToRemove, cancellationToken);
            if (participantsToAdd.Count > 0)
                await participantRepository.AddRangeAsync(participantsToAdd, cancellationToken);
        }

        return request.CourseId;
    }
}
