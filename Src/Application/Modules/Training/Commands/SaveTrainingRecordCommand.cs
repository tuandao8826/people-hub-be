using Application.Common.Interfaces.Persistence;
using Application.Modules.Training.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Commands;

public record SaveTrainingRecordCommand(Guid StaffMemberId, Guid SubjectId, DateTimeOffset? TrainingDate) : IRequest;

public class SaveTrainingRecordHandler(IUnitOfWork unitOfWork) : IRequestHandler<SaveTrainingRecordCommand>
{
    public async Task Handle(SaveTrainingRecordCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<TrainingRecord>();
        var existing = await repository.Find(x => x.StaffMemberId == request.StaffMemberId && x.SubjectId == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);
        if (request.TrainingDate is null)
        {
            if (existing is not null) await repository.DeleteAsync(existing, cancellationToken);
            return;
        }

        if (existing is null)
            await repository.AddAsync(new TrainingRecord
            {
                StaffMemberId = request.StaffMemberId,
                SubjectId = request.SubjectId,
                TrainingDate = request.TrainingDate.Value
            }, cancellationToken);
        else
        {
            existing.TrainingDate = request.TrainingDate.Value;
            await repository.UpdateAsync(existing, cancellationToken);
        }
    }
}
