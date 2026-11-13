using FluentValidation;

namespace Application.Modules.Subjects.Commands.Update;

public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(x => x.Data)
            .SetValidator(new UpdateSubjectRequestValidator());
    }
}

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(x => x.SubjectCode)
            .NotEmpty().WithMessage("Mã môn học không được để trống.")
            .MaximumLength(50).WithMessage("Mã môn học không được vượt quá 50 ký tự.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên môn học không được để trống.")
            .MaximumLength(255).WithMessage("Tên môn học không được vượt quá 255 ký tự.");

        RuleFor(x => x.CycleValue)
            .GreaterThan(0).WithMessage("Giá trị chu kỳ đào tạo phải lớn hơn 0.");

        RuleFor(x => x.SubjectCycleUnit)
            .IsInEnum().WithMessage("Đơn vị chu kỳ không hợp lệ.");
    }
}