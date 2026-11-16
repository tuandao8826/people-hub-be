using Application.Common.Interfaces.Persistence;
using Application.Modules.FlightSchedules.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FlightSchedules.Commands;

public record DeleteFlightScheduleCommand(Guid Id) : IRequest<bool>;
public class DeleteFlightScheduleHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteFlightScheduleCommand, bool>
{
    public async Task<bool> Handle(DeleteFlightScheduleCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<FlightSchedule>();
        var entity = await repository.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity is null) return false;
        await repository.DeleteAsync(entity, cancellationToken);
        return true;
    }
}
