using Application.Modules.Subjects.Bases.Responses;
using MediatR;

namespace Application.Modules.Subjects.Queries.Detail;

public record class GetSubjectDetailQuery(Guid SubjectId) : IRequest<SubjectDefaultResponse>;