using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Training.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public class GetTrainingNotificationsQuery : PaginationParams, IRequest<TrainingNotificationsPageResponse>
{
}

public record TrainingNotificationResponse(
    Guid Id,
    TrainingNotificationType Type,
    int EmployeeCount,
    bool IsRead,
    DateTimeOffset CreatedAt);

public record TrainingNotificationsPageResponse(
    IReadOnlyList<TrainingNotificationResponse> PageItems,
    PageInfo PageInfo,
    int UnreadCount);

public class GetTrainingNotificationsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTrainingNotificationsQuery, TrainingNotificationsPageResponse>
{
    public async Task<TrainingNotificationsPageResponse> Handle(
        GetTrainingNotificationsQuery request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<TrainingNotification>();
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);
        var query = repository.Find().AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var unreadCount = await repository.Find(notification => !notification.IsRead)
            .CountAsync(cancellationToken);
        var pageItems = await query.OrderByDescending(notification => notification.CreatedAt)
            .ThenByDescending(notification => notification.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(notification => new TrainingNotificationResponse(
                notification.Id,
                notification.Type,
                notification.EmployeeCount,
                notification.IsRead,
                notification.CreatedAt))
            .ToListAsync(cancellationToken);

        return new TrainingNotificationsPageResponse(
            pageItems,
            new PageInfo(page, pageSize, pageItems.Count, totalCount),
            unreadCount);
    }
}
