using Api.Common.ControllerWrappers;
using Application.Common.ApiWrapper;
using Application.Modules.FlightSchedules.Commands;
using Application.Modules.FlightSchedules.Contracts;
using Application.Modules.FlightSchedules.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin.FlightSchedules;

public class FlightSchedulesController(IMediator mediator) : AdminBaseController
{
    [HttpGet("Statistics"), Authorize]
    public async Task<ActionResult<SuccessResultResponse<FlightStatisticsResponse>>> Statistics(
        [FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo, CancellationToken ct) =>
        ResultResponse(await mediator.Send(new GetFlightStatisticsQuery(dateFrom, dateTo), ct), "Thống kê lịch bay theo ngày.");

    [HttpGet, Authorize]
    public async Task<ActionResult<SuccessResultResponse<Application.Common.Responses.PaginationResponse<FlightScheduleListItemResponse>>>> Search([FromQuery] SearchFlightSchedulesQuery query, CancellationToken ct) => ResultResponse(await mediator.Send(query, ct), "Tìm kiếm lịch bay thành công.");
    [HttpGet("{id:guid}"), Authorize]
    public async Task<ActionResult<SuccessResultResponse<FlightScheduleResponse>>> Detail(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFlightScheduleDetailQuery(id), ct);
        return result is null ? NotFound() : ResultResponse(result, "Chi tiết lịch bay.");
    }
    [HttpPost, Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> Create([FromBody] FlightScheduleRequest request, CancellationToken ct) => ResultResponse(await mediator.Send(new SaveFlightScheduleCommand(null, request), ct), "Đã tạo lịch bay.");
    [HttpPut("{id:guid}"), Authorize]
    public async Task<ActionResult<SuccessResultResponse<Guid>>> Update(Guid id, [FromBody] FlightScheduleRequest request, CancellationToken ct) => ResultResponse(await mediator.Send(new SaveFlightScheduleCommand(id, request), ct), "Đã cập nhật lịch bay.");
    [HttpDelete("{id:guid}"), Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) => await mediator.Send(new DeleteFlightScheduleCommand(id), ct) ? NoContent() : NotFound();
    [HttpPost("Import/Preview"), Authorize, RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<SuccessResultResponse<Application.Modules.FlightSchedules.Contracts.FlightImportSummary>>> Preview([FromForm] IFormFile file, CancellationToken ct) => await Import(file, true, ct);
    [HttpPost("Import"), Authorize, RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<SuccessResultResponse<Application.Modules.FlightSchedules.Contracts.FlightImportSummary>>> Import([FromForm] IFormFile file, CancellationToken ct) => await Import(file, false, ct);
    private async Task<ActionResult<SuccessResultResponse<Application.Modules.FlightSchedules.Contracts.FlightImportSummary>>> Import(IFormFile file, bool preview, CancellationToken ct)
    {
        if (file is null || file.Length == 0) throw new ArgumentException("Vui lòng chọn file CSV hoặc XLSX.");
        if (!new[] { ".csv", ".xlsx" }.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase)) throw new ArgumentException("Chỉ hỗ trợ file .csv hoặc .xlsx.");
        await using var stream = new MemoryStream(); await file.CopyToAsync(stream, ct);
        var result = await mediator.Send(new ImportFlightSchedulesCommand(file.FileName, stream.ToArray(), preview), ct);
        return ResultResponse(result, preview ? "Đã đọc trước dữ liệu lịch bay." : "Đã nhập lịch bay.");
    }
}
