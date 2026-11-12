using Application.Common.Interfaces.Persistence;
using Application.Modules.Courses.Entities;
using AutoMapper;
using MediatR;

namespace Application.Modules.Courses.Commands.Create;

public class CreateCourseHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCourseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = mapper.Map<Course>(request);
        await unitOfWork.Repository<Course>().AddAsync(course, cancellationToken);

        // Thêm danh sách nhân viên tham gia nếu có truyền lên
        if (request.StaffMemberIds != null && request.StaffMemberIds.Count != 0)
        {
            var participants = request.StaffMemberIds.Select(staffId => new CourseParticipant
            {
                CourseId = course.Id,
                StaffMemberId = staffId,
                IsCompleted = false
            }).ToList();

            await unitOfWork.Repository<CourseParticipant>().AddRangeAsync(participants, cancellationToken);
        }

        return course.Id;
    }
}