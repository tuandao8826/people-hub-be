using Application.Modules.Categories.Bases.Requests;
using Application.Modules.Categories.Bases.Responses;
using MediatR;

namespace Application.Modules.Categories.Commands.Update;

public record class UpdateCategoryCommand(Guid CategoryId, UpdateCategoryRequest Data = null!) : IRequest<CategoryDefaultResponse>;

public class UpdateCategoryRequest : CategoryRequest;