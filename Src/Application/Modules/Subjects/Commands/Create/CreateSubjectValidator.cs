using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Entites;
using FluentValidation;

namespace Application.Modules.Subjects.Commands.Create;

public class CreateSubjectValidator : AbstractValidator<CreateSubjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubjectValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.SubjectCode)
            .NotEmpty().WithMessage("Mã môn học không được để trống.")
            .MaximumLength(50).WithMessage("Mã môn học không được vượt quá 50 ký tự.")
            /*.MustAsync(BeUniqueSubjectCode).WithMessage("Mã môn học này đã tồn tại trong hệ thống.")*/;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên môn học không được để trống.")
            .MaximumLength(255).WithMessage("Tên môn học không được vượt quá 255 ký tự.");

        RuleFor(x => x.CycleValue)
            .GreaterThan(0).WithMessage("Giá trị chu kỳ đào tạo phải lớn hơn 0.");

        RuleFor(x => x.SubjectCycleUnit)
            .IsInEnum().WithMessage("Đơn vị chu kỳ không hợp lệ.");
    }

    //private async Task<bool> BeUniqueSubjectCode(string subjectCode, CancellationToken cancellationToken)
    //{
    //    var existingSubject = await _unitOfWork.Repository<Subject>()
    //        .GetQueryable()
    //        .AnyAsync(x => x.SubjectCode == subjectCode, cancellationToken);

    //    return !existingSubject;
    //}
}