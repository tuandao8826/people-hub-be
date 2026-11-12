using Application.Common.Interfaces.Persistence;
using Application.Modules.Courses.Entities;
using Application.Modules.Subjects.Entites;
using FluentValidation;

namespace Application.Modules.Courses.Commands.Create;

public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        //RuleFor(x => x.CourseCode)
        //    .NotEmpty().WithMessage("Mã khóa học không được để trống.")
        //    .MaximumLength(50).WithMessage("Mã khóa học không được vượt quá 50 ký tự.")
        //    .Must(BeUniqueCourseCode).WithMessage("Mã khóa học này đã tồn tại trong hệ thống.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khóa học không được để trống.")
            .MaximumLength(255).WithMessage("Tên khóa học không được vượt quá 255 ký tự.");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("Phải chọn môn học cho khóa học.")
            .Must(SubjectExists).WithMessage("Môn học được chọn không tồn tại.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Ngày kết thúc không được để trống.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
    }

    private bool BeUniqueCourseCode(string courseCode)
    {
        return _unitOfWork.Repository<Course>()
            .Any(x => x.CourseCode == courseCode);
    }

    private bool SubjectExists(Guid subjectId)
    {
        return _unitOfWork.Repository<Subject>()
            .Any(x => x.Id == subjectId);
    }
}