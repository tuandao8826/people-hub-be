using Application.Common.Interfaces.Persistence;
using Application.Common.Responses;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public class SearchTrainingMatrixHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SearchTrainingMatrixQuery, PaginationResponse<TrainingEmployeeRowResponse>>
{
    public async Task<PaginationResponse<TrainingEmployeeRowResponse>> Handle(
        SearchTrainingMatrixQuery request, CancellationToken cancellationToken)
    {
        var staffQuery = unitOfWork.Repository<StaffMember>().Find().AsNoTracking();
        var keyword = request.Search?.Keyword?.Trim();
        if (!string.IsNullOrWhiteSpace(keyword))
            staffQuery = staffQuery.Where(x => EF.Functions.ILike(x.Code, $"%{keyword}%") ||
                                               EF.Functions.ILike(x.Name, $"%{keyword}%"));

        if (request.SubjectId.HasValue)
        {
            var trainingRecordQuery = unitOfWork.Repository<TrainingRecord>().Find();
            staffQuery = staffQuery.Where(member => trainingRecordQuery.Any(record =>
                record.StaffMemberId == member.Id && record.SubjectId == request.SubjectId.Value));
        }

        if (request.OnlyExpired || request.TrainingStatus.HasValue)
        {
            var selectedStatus = request.OnlyExpired
                ? TrainingRecordStatus.Expired
                : request.TrainingStatus!.Value;
            var candidateRecords = unitOfWork.Repository<TrainingRecord>().Find();
            if (request.SubjectId is Guid subjectId)
                candidateRecords = candidateRecords.Where(record => record.SubjectId == subjectId);

            var matchingStaffIds = TrainingExpiry.SelectExpirations(candidateRecords, unitOfWork.Repository<Subject>().Find())
                .Filter(selectedStatus, TrainingExpiry.Today)
                .Select(record => record.StaffMemberId);
            staffQuery = staffQuery.Where(member => matchingStaffIds.Contains(member.Id));
        }

        var total = await staffQuery.CountAsync(cancellationToken);
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var staff = await staffQuery.OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.Code, x.Name })
            .ToListAsync(cancellationToken);

        var staffIds = staff.Select(x => x.Id).ToArray();
        var records = await unitOfWork.Repository<TrainingRecord>().Find(x => staffIds.Contains(x.StaffMemberId))
            .AsNoTracking().Select(x => new { x.StaffMemberId, x.SubjectId, x.TrainingDate })
            .ToListAsync(cancellationToken);
        var subjectIds = records.Select(x => x.SubjectId).Distinct().ToArray();
        var subjects = await unitOfWork.Repository<Subject>().Find(x => subjectIds.Contains(x.Id))
            .AsNoTracking().ToDictionaryAsync(x => x.Id, cancellationToken);

        var rows = staff.Select(member => new TrainingEmployeeRowResponse(
            member.Id,
            member.Code,
            member.Name,
            records.Where(x => x.StaffMemberId == member.Id && subjects.ContainsKey(x.SubjectId))
                .Select(record => new EmployeeSubjectTrainingResponse(
                    record.SubjectId,
                    record.TrainingDate,
                    TrainingExpiry.AddCycle(record.TrainingDate, subjects[record.SubjectId])))
                .ToList())).ToList();

        return new PaginationResponse<TrainingEmployeeRowResponse>(rows,
            new PageInfo(page, pageSize, rows.Count, total));
    }

}
