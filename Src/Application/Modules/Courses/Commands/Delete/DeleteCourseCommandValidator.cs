using Application.Common.Interfaces.Persistence;
using Application.Common.Requests;
using Application.Modules.Courses.Entities;
using FluentValidation;

namespace Application.Modules.Courses.Commands.Delete;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new MultipleIdentiferValidator<Course, Guid>(unitOfWork));
    }
}