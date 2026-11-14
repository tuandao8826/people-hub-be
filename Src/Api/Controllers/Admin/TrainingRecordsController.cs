using Api.Common.ControllerWrappers;
using Api.TrainingNotifications;
using Application.Common.ApiWrapper;
using Application.Common.Responses;
using Application.Modules.Training.Commands;
using Application.Modules.Training.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

public class TrainingRecordsController(IMediator mediator, TrainingNotificationSnapshot notificationSnapshot) : AdminBaseController
{
    [HttpPost("Import")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<ImportTrainingResult>>> ImportAsync(
        [FromBody] ImportTrainingCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        notificationSnapshot.Invalidate();
        return ResultResponse(result, "Đã nhập hồ sơ đào tạo.");
    }

    [HttpGet("Notifications")]
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<SuccessResultResponse<TrainingNotificationsResponse>>> NotificationsAsync(
        CancellationToken cancellationToken)
    {
        var notifications = notificationSnapshot.GetToday()
            ?? await mediator.Send(new GetTrainingNotificationsQuery(), cancellationToken);
        return ResultResponse(notifications, "Thông báo hạn đào tạo.");
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<PaginationResponse<TrainingEmployeeRowResponse>>>> SearchAsync(
        [FromQuery] SearchTrainingMatrixQuery request, CancellationToken cancellationToken)
    {
        return ResultResponse(await mediator.Send(request, cancellationToken), "Tìm kiếm hồ sơ đào tạo thành công.");
    }

    [HttpPut("{staffMemberId:guid}/{subjectId:guid}")]
    [Authorize]
    public async Task<ActionResult<SuccessResultResponse<object>>> SaveAsync(
        Guid staffMemberId, Guid subjectId, [FromBody] SaveTrainingRecordRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new SaveTrainingRecordCommand(staffMemberId, subjectId, request.TrainingDate), cancellationToken);
        notificationSnapshot.Invalidate();
        return ResultResponse("Đã lưu hồ sơ đào tạo.");
    }
}

public record SaveTrainingRecordRequest(DateTimeOffset? TrainingDate);
