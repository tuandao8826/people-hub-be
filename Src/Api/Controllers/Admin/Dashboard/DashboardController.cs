using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Modules.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.Dashboard;

public class DashboardController(IMediator mediator) : AdminBaseController
{
    [HttpGet("Statistics")]
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<SuccessResultResponse<DashboardStatisticsResponse>>> StatisticsAsync(
        CancellationToken cancellationToken)
    {
        var statistics = await mediator.Send(new GetDashboardStatisticsQuery(), cancellationToken);
        return ResultResponse(statistics, "Thống kê tổng quan.");
    }
}
