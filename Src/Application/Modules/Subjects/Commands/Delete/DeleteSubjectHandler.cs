using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Courses.Entities;
using Application.Modules.Subjects.Commands;
using Application.Modules.Subjects.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Subjects.Commands.Delete;

public class DeleteSubjectHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSubjectCommand, MultipleIdentiferResponse>
{
    public async Task<MultipleIdentiferResponse> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Ids ?? [];
        var isUsedByCourse = await unitOfWork.Repository<Course>()
            .AnyAsync(course => ids.Contains(course.SubjectId), cancellationToken);
        if (isUsedByCourse)
            throw SubjectConstraintErrors.InUse();

        try
        {
            await unitOfWork.Repository<Subject>()
                .Find(subject => ids.Contains(subject.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw SubjectConstraintErrors.InUse(exception);
        }

        return new MultipleIdentiferResponse(ids);
    }
}
