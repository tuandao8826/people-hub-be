using Application.Common.Responses;
using Application.Modules.Subjects.Bases.Responses;
using MediatR;

namespace Application.Modules.Subjects.Queries.Search;

public class SearchSubjectQuery : PaginationParams, IRequest<PaginationResponse<SubjectDefaultResponse>>;