using Application.Common.Responses;
using Application.Modules.Staff.Bases.Responses;
using MediatR;

namespace Application.Modules.Staff.Queries.Search;

public class SearchStaffQuery : PaginationParams, IRequest<PaginationResponse<StaffResponse>>;
