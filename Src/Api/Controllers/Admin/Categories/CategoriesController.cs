using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Categories.Commands.Create;
using Application.Modules.Categories.Commands.DeleteMany;
using Application.Modules.Categories.Commands.Update;
using Application.Modules.Categories.Entities;
using Application.Modules.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Categories;

public class CategoriesController(IMediator mediator) : AdminBaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<CategoryDefaultResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(new GetCategoryDetailQuery(id), cancellationToken), Message<Category>.Detail());
    }

    [HttpPost]
    public async Task<ActionResult<SuccessResultResponse<CategoryDefaultResponse>>> CreateAsync(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Category>.Create());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<CategoryDefaultResponse>>> UpdateAsync([FromRoute] Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(new UpdateCategoryCommand(id, request), cancellationToken), Message<Category>.Update());
    }

    [HttpPost("DeleteMany")]
    public async Task<ActionResult<SuccessResultResponse<MultipleIdentiferResponse>>> DeleteManyAsync(DeleteCategoriesCommand request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Category>.Delete());
    }
}
