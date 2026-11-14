using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public record GetTrainingNotificationsQuery : IRequest<TrainingNotificationsResponse>;

public record TrainingNotificationsResponse(int ExpiringSoon, int ExpiresToday);

public class GetTrainingNotificationsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTrainingNotificationsQuery, TrainingNotificationsResponse>
{
    public async Task<TrainingNotificationsResponse> Handle(GetTrainingNotificationsQuery request, CancellationToken cancellationToken)
    {
        var today = TrainingExpiry.Today;
        var records = TrainingExpiry.SelectExpirations(
            unitOfWork.Repository<TrainingRecord>().Find(), unitOfWork.Repository<Subject>().Find());
        var expiringSoon = await records.Filter(TrainingRecordStatus.ExpiringSoon, today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);
        var expiresToday = await records.Filter(TrainingRecordStatus.ExpiresToday, today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);
        return new TrainingNotificationsResponse(expiringSoon, expiresToday);
    }
}
