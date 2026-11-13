using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Entites;
using Application.Modules.Staff.Entities;
using FluentValidation;

namespace Application.Modules.Courses.Commands.Update;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Data)
            .SetValidator(new UpdateCourseRequestValidator(unitOfWork));
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Mã khóa học không được để trống.")
            .MaximumLength(50).WithMessage("Mã khóa học không được vượt quá 50 ký tự.");

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

        RuleFor(x => x.StaffMemberIds)
            .Must(ids => ids is null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Danh sách nhân viên tham gia không được có ID trùng.")
            .Must(StaffMembersExist)
            .WithMessage("Một hoặc nhiều nhân viên tham gia không tồn tại.");
    }

    private bool SubjectExists(Guid subjectId)
    {
        return _unitOfWork.Repository<Subject>()
            .Any(x => x.Id == subjectId);
    }

    private bool StaffMembersExist(ICollection<Guid>? staffMemberIds)
    {
        if (staffMemberIds is null || staffMemberIds.Count == 0) return true;
        return _unitOfWork.Repository<StaffMember>()
            .Find(staff => staffMemberIds.Contains(staff.Id))
            .Count() == staffMemberIds.Count;
    }
}
