using Application.Modules.Subjects.Entites;
using Core.Common.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Modules.Subjects.Commands.Create;

public class CreateSubjectCommand : IRequest<Guid> 
{
    public string SubjectCode { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public int SubjectCycleValue { get; set; }

    public SubjectCycleUnit SubjectCycleUnit { get; set; }

    public OperationStatus OperationStatus { get; set; } = OperationStatus.Active;
}
