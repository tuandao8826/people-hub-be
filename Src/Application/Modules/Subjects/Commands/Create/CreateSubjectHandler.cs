using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Commands;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Subjects.Commands.Create;

public class CreateSubjectHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateSubjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Subject>();
        request.SubjectCode = request.SubjectCode.Trim();
        var normalizedCode = request.SubjectCode.ToLower();
        var codeExists = await repository.AnyAsync(
            subject => subject.SubjectCode.Trim().ToLower() == normalizedCode,
            cancellationToken);
        if (codeExists)
            throw SubjectConstraintErrors.DuplicateCode(request.SubjectCode);

        request.Name = request.Name.Trim();
        var normalizedName = request.Name.ToLower();
        var nameExists = await repository.AnyAsync(
            subject => subject.Name.Trim().ToLower() == normalizedName,
            cancellationToken);
        if (nameExists)
            throw SubjectConstraintErrors.DuplicateName(request.Name);

        var subject = mapper.Map<Subject>(request);
        try
        {
            await repository.AddAsync(subject, cancellationToken);
        }
        catch (DbUpdateException exception) when (SubjectConstraintErrors.IsDuplicateCode(exception))
        {
            throw SubjectConstraintErrors.DuplicateCode(request.SubjectCode, exception);
        }
        catch (DbUpdateException exception) when (SubjectConstraintErrors.IsDuplicateName(exception))
        {
            throw SubjectConstraintErrors.DuplicateName(request.Name, exception);
        }

        return subject.Id;
    }
}
