using Application.Modules.Training.Queries;
using Application.Modules.Training.Commands;
using MediatR;

namespace Api.TrainingNotifications;

internal sealed class TrainingNotificationJob(
    IServiceScopeFactory scopeFactory,
    TrainingNotificationSchedule schedule,
    ILogger<TrainingNotificationJob> logger) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            var nextRun = schedule.GetNextRun(now);
            logger.LogInformation("[Thông báo đào tạo] Lần chạy kế tiếp lúc {ScheduledAt:dd/MM/yyyy HH:mm zzz}.", nextRun);
            await Task.Delay(nextRun - now, stoppingToken);

            var scheduledDate = TrainingNotificationSchedule.GetLocalDate(nextRun);
            while (TrainingNotificationSchedule.GetLocalDate(DateTimeOffset.UtcNow) == scheduledDate)
            {
                try
                {
                    logger.LogInformation("[Thông báo đào tạo] Bắt đầu kiểm tra hạn đào tạo ngày {ScheduledDate:dd/MM/yyyy}.", scheduledDate);
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    var counts = await sender.Send(new GetTrainingNotificationCountsQuery(), stoppingToken);
                    var created = await sender.Send(new GenerateTrainingNotificationsCommand(
                        scheduledDate, counts.Expired, counts.ExpiringSoon), stoppingToken);
                    logger.LogInformation(
                        "[Thông báo đào tạo] Quét xong. Đã hết hạn: {Expired} nhân viên; sắp hết hạn trong 14 ngày: {ExpiringSoon} nhân viên; tạo mới {Created} thông báo.",
                        counts.Expired, counts.ExpiringSoon, created);
                    break;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "[Thông báo đào tạo] Chạy thất bại. Sẽ thử lại sau 1 phút.");
                }

                await Task.Delay(RetryDelay, stoppingToken);
            }
        }
    }
}
