using Application.Modules.Categories.Bases.Requests;
using Application.Modules.Categories.Bases.Responses;
using MediatR;

namespace Application.Modules.Categories.Commands.Create;

public class CreateCategoryCommand : CategoryRequest, IRequest<CategoryDefaultResponse>
{
    /// <summary>
    /// Danh mục cha
    /// </summary>
    public Guid? ParentId { get; set; }
}
