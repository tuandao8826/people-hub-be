using Application.Modules.Categories.Bases.Responses;
using MediatR;

namespace Application.Modules.Categories.Queries.Detail;

public record class GetCategoryDetailQuery(Guid CategoryId) : IRequest<CategoryDefaultResponse>;
