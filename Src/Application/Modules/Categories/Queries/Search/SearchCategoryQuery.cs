using Application.Common.Responses;
using Application.Modules.Categories.Bases.Responses;
using MediatR;

namespace Application.Modules.Categories.Queries.Search;

public class SearchCategoryQuery : PaginationParams, IRequest<PaginationResponse<CategoryDefaultResponse>>;
