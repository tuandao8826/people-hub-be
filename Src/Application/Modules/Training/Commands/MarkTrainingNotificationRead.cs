using Application.Common.Interfaces.Persistence;
using Application.Modules.Training.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Commands;

public record MarkTrainingNotificationReadCommand(Guid Id) : IRequest<bool>;

public class MarkTrainingNotificationReadHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkTrainingNotificationReadCommand, bool>
{
    public async Task<bool> Handle(
        MarkTrainingNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<TrainingNotification>();
        var notification = await repository.Find(item => item.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (notification is null) return false;
        if (notification.IsRead) return true;

        notification.IsRead = true;
        await repository.UpdateAsync(notification, cancellationToken);
        return true;
    }
}
