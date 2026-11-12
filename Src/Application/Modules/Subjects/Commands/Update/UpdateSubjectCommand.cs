using Application.Modules.Subjects.Entites;
using Core.Common.Enums;
using MediatR;

namespace Application.Modules.Subjects.Commands.Update;

public record class UpdateSubjectCommand(Guid SubjectId, UpdateSubjectRequest Data = null!) : IRequest<Guid>;

public class UpdateSubjectRequest
{
    public string SubjectCode { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public int CycleValue { get; set; }

    public SubjectCycleUnit CycleUnit { get; set; }

    public OperationStatus OperationStatus { get; set; }
}