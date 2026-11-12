using Application.Common.Interfaces.Persistence;
using Application.Common.Requests;
using Application.Modules.Subjects.Entites;
using FluentValidation;

namespace Application.Modules.Subjects.Commands.Delete;

public class DeleteSubjectCommandValidator : AbstractValidator<DeleteSubjectCommand>
{
    public DeleteSubjectCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new MultipleIdentiferValidator<Subject, Guid>(unitOfWork));
    }
}
