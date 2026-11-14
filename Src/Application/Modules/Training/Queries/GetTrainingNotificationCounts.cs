using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using Core.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public record GetTrainingNotificationCountsQuery : IRequest<TrainingNotificationCounts>;
public record TrainingNotificationCounts(int Expired, int ExpiringSoon);

public class GetTrainingNotificationCountsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTrainingNotificationCountsQuery, TrainingNotificationCounts>
{
    public async Task<TrainingNotificationCounts> Handle(
        GetTrainingNotificationCountsQuery request, CancellationToken cancellationToken)
    {
        var activeStaffIds = unitOfWork.Repository<StaffMember>()
            .Find(member => member.Status == StaffStatus.Active)
            .Select(member => member.Id);
        var activeSubjects = unitOfWork.Repository<Subject>()
            .Find(subject => subject.OperationStatus == OperationStatus.Active);
        var records = TrainingExpiry.SelectExpirations(
                unitOfWork.Repository<TrainingRecord>().Find(), activeSubjects)
            .Where(expiration => activeStaffIds.Contains(expiration.StaffMemberId));
        var today = TrainingExpiry.Today;

        var expired = await records.Filter(TrainingRecordStatus.Expired, today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);
        var expiringSoon = await records.Filter(TrainingRecordStatus.ExpiringSoon, today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);

        return new TrainingNotificationCounts(expired, expiringSoon);
    }
}
