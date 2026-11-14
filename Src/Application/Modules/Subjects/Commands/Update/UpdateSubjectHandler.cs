using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Commands;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Modules.Subjects.Commands.Update;

public class UpdateSubjectHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateSubjectCommand, Guid>
{
    public async Task<Guid> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Subject>();
        var subject = await repository.Find(item => item.Id == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException($"Không tìm thấy môn học với ID: {request.SubjectId}", HttpStatusCode.NotFound);

        request.Data.SubjectCode = request.Data.SubjectCode.Trim();
        var normalizedCode = request.Data.SubjectCode.ToLower();
        if (!string.Equals(subject.SubjectCode.Trim(), request.Data.SubjectCode, StringComparison.OrdinalIgnoreCase))
        {
            var codeExists = await repository.AnyAsync(
                item => item.Id != request.SubjectId && item.SubjectCode.Trim().ToLower() == normalizedCode,
                cancellationToken);
            if (codeExists)
                throw SubjectConstraintErrors.DuplicateCode(request.Data.SubjectCode);
        }

        request.Data.Name = request.Data.Name.Trim();
        var normalizedName = request.Data.Name.ToLower();
        if (!string.Equals(subject.Name.Trim(), request.Data.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await repository.AnyAsync(
                item => item.Id != request.SubjectId && item.Name.Trim().ToLower() == normalizedName,
                cancellationToken);
            if (nameExists)
                throw SubjectConstraintErrors.DuplicateName(request.Data.Name);
        }

        mapper.Map(request.Data, subject);
        try
        {
            await repository.UpdateAsync(subject, cancellationToken);
        }
        catch (DbUpdateException exception) when (SubjectConstraintErrors.IsDuplicateCode(exception))
        {
            throw SubjectConstraintErrors.DuplicateCode(request.Data.SubjectCode, exception);
        }
        catch (DbUpdateException exception) when (SubjectConstraintErrors.IsDuplicateName(exception))
        {
            throw SubjectConstraintErrors.DuplicateName(request.Data.Name, exception);
        }

        return request.SubjectId;
    }
}
