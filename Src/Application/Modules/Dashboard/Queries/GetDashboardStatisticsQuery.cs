using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Dashboard.Queries;

public sealed record GetDashboardStatisticsQuery : IRequest<DashboardStatisticsResponse>;

public sealed record DashboardStatisticsResponse(
    int TotalEmployees,
    int ActiveEmployees,
    int TotalSubjects);

public sealed class GetDashboardStatisticsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsResponse>
{
    public async Task<DashboardStatisticsResponse> Handle(
        GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        var staff = unitOfWork.Repository<StaffMember>().Find().AsNoTracking();
        var totalEmployees = await staff.CountAsync(cancellationToken);
        var activeEmployees = await staff.CountAsync(x => x.Status == StaffStatus.Active, cancellationToken);
        var totalSubjects = await unitOfWork.Repository<Subject>().Find()
            .AsNoTracking().CountAsync(cancellationToken);

        return new DashboardStatisticsResponse(totalEmployees, activeEmployees, totalSubjects);
    }
}
