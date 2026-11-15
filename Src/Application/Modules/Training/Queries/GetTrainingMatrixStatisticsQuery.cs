using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using Core.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public sealed record GetTrainingMatrixStatisticsQuery : IRequest<TrainingMatrixStatisticsResponse>;

public sealed record TrainingMatrixStatisticsResponse(
    int ActiveEmployees,
    int ActiveSubjects,
    int ExpiredEmployees,
    int ExpiringEmployees);

public sealed class GetTrainingMatrixStatisticsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTrainingMatrixStatisticsQuery, TrainingMatrixStatisticsResponse>
{
    public async Task<TrainingMatrixStatisticsResponse> Handle(
        GetTrainingMatrixStatisticsQuery request, CancellationToken cancellationToken)
    {
        var activeStaffQuery = unitOfWork.Repository<StaffMember>().Find(x => x.Status == StaffStatus.Active).AsNoTracking();
        var activeSubjectQuery = unitOfWork.Repository<Subject>()
            .Find(x => x.OperationStatus == OperationStatus.Active).AsNoTracking();
        var trainingRecords = unitOfWork.Repository<TrainingRecord>().Find().AsNoTracking();

        var activeEmployees = await activeStaffQuery.CountAsync(cancellationToken);
        var activeSubjects = await activeSubjectQuery.CountAsync(cancellationToken);
        var expirations = TrainingExpiry.SelectExpirations(trainingRecords, activeSubjectQuery)
            .Where(record => activeStaffQuery.Any(staff => staff.Id == record.StaffMemberId));
        var expiredEmployees = await expirations
            .Filter(TrainingRecordStatus.Expired, TrainingExpiry.Today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);
        var expiringEmployees = await expirations
            .Filter(TrainingRecordStatus.ExpiringSoon, TrainingExpiry.Today)
            .Select(record => record.StaffMemberId).Distinct().CountAsync(cancellationToken);

        return new TrainingMatrixStatisticsResponse(activeEmployees, activeSubjects, expiredEmployees, expiringEmployees);
    }
}
