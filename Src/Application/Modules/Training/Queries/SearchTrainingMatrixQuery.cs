using Application.Common.Responses;
using MediatR;

namespace Application.Modules.Training.Queries;

public class SearchTrainingMatrixQuery : PaginationParams, IRequest<PaginationResponse<TrainingEmployeeRowResponse>>
{
    public bool OnlyExpired { get; init; }
    public Guid? SubjectId { get; init; }
    public TrainingRecordStatus? TrainingStatus { get; init; }
}

public enum TrainingRecordStatus
{
    Valid = 1,
    Expired = 2,
    ExpiringSoon = 3,
    ExpiresToday = 4
}

public record TrainingEmployeeRowResponse(
    Guid StaffMemberId,
    string StaffCode,
    string StaffName,
    IReadOnlyList<EmployeeSubjectTrainingResponse> Subjects);

public record EmployeeSubjectTrainingResponse(
    Guid SubjectId,
    DateTimeOffset TrainingDate,
    DateTimeOffset ExpirationDate);
