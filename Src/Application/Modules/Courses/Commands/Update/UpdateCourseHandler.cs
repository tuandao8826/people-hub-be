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
        await unitOfWork.Repository<Course>().UpdateAsync(course, cancellationToken);

        return request.CourseId;
    }
}