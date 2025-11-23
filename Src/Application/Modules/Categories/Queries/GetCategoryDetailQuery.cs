using Application.Modules.Categories.Bases.Responses;
using MediatR;

namespace Application.Modules.Categories.Queries;

public record class GetCategoryDetailQuery(Guid CategoryId) : IRequest<CategoryDefaultResponse>;
