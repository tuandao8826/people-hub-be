using Application.Modules.Subjects.Entites;
using Core.Bases;
using Core.Common.Enums;

namespace Application.Modules.Subjects.Bases.Responses;

public class SubjectDefaultResponse : BaseEntity
{
    public string SubjectCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int CycleValue { get; set; }
    public SubjectCycleUnit SubjectCycleUnit { get; set; }
    public OperationStatus OperationStatus { get; set; }
}
