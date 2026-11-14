using Application.Common.Interfaces.Persistence;
using Application.Modules.Training.Entities;
using MediatR;

namespace Application.Modules.Training.Commands;

public record GenerateTrainingNotificationsCommand(
    DateOnly ScanDate,
    int ExpiredEmployeeCount,
    int ExpiringSoonEmployeeCount) : IRequest<int>;

public class GenerateTrainingNotificationsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateTrainingNotificationsCommand, int>
{
    public async Task<int> Handle(
        GenerateTrainingNotificationsCommand request, CancellationToken cancellationToken)
    {
        var candidates = new[]
        {
            (Type: TrainingNotificationType.Expired, Count: request.ExpiredEmployeeCount),
            (Type: TrainingNotificationType.ExpiringSoon, Count: request.ExpiringSoonEmployeeCount)
        }.Where(candidate => candidate.Count > 0).ToArray();
        if (candidates.Length == 0) return 0;

        var repository = unitOfWork.Repository<TrainingNotification>();
        var newNotifications = candidates
            .Select(candidate => new TrainingNotification
            {
                ScanDate = request.ScanDate,
                Type = candidate.Type,
                EmployeeCount = candidate.Count,
                IsRead = false
            })
            .ToArray();

        if (newNotifications.Length == 0) return 0;
        await repository.AddRangeAsync(newNotifications, cancellationToken);
        return newNotifications.Length;
    }
}
