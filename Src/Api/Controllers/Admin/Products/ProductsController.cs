using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Common.Responses;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Commands.Create;
using Application.Modules.Products.Commands.DeleteMany;
using Application.Modules.Products.Commands.Update;
using Application.Modules.Products.Entities;
using Application.Modules.Products.Queries.Detail;
using Application.Modules.Products.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Products;

public class ProductsController(IMediator mediator) : AdminBaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<ProductDefaultResponse>>> GetDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(new GetProductDetailQuery(id), cancellationToken), Message<Product>.Detail());
    }

    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<ProductDefaultResponse>>>> SearchAsync([FromQuery] SearchProductQuery request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Product>.Search());
    }

    [HttpPost]
    public async Task<ActionResult<SuccessResultResponse<ProductDefaultResponse>>> CreateAsync([FromForm] CreateProductCommand request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Product>.Create());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SuccessResultResponse<ProductDefaultResponse>>> UpdateAsync([FromRoute] Guid id, [FromForm] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(new UpdateProductCommand(id, request), cancellationToken), Message<Product>.Update());
    }

    [HttpPost("DeleteMany")]
    public async Task<ActionResult<SuccessResultResponse<MultipleIdentiferResponse>>> DeleteAsync([FromBody] DeleteProductsCommand request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Product>.Delete());
    }
}
