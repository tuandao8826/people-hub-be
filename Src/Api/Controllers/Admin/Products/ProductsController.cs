using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Modules.Products.Bases.Responses.Products;
using Application.Modules.Products.Entities;
using Application.Modules.Products.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Products;

public class ProductsController(IMediator mediator) : AdminBaseController
{
    [HttpGet]
    public async Task<ActionResult<SuccessResultResponse<List<ProductDefaultResponse>>>> SearchAsync([FromQuery] SearchProductQuery request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), Message<Product>.Search());
    } 
}
