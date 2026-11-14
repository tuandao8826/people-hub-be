using Application.Modules.Training.Queries;
using MediatR;

namespace Api.TrainingNotifications;

internal sealed class TrainingNotificationJob(
    IServiceScopeFactory scopeFactory,
    TrainingNotificationSchedule schedule,
    TrainingNotificationSnapshot snapshot,
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
                    var version = snapshot.GetVersion();
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    var notifications = await sender.Send(new GetTrainingNotificationsQuery(), stoppingToken);
                    if (snapshot.TryPublish(scheduledDate, version, notifications))
                    {
                        logger.LogInformation(
                            "[Thông báo đào tạo] Chạy thành công. {ExpiringSoon} nhân viên hết hạn trong 14 ngày tới, {ExpiresToday} nhân viên hết hạn hôm nay.",
                            notifications.ExpiringSoon, notifications.ExpiresToday);
                        break;
                    }

                    logger.LogInformation("[Thông báo đào tạo] Dữ liệu vừa thay đổi. Sẽ thử lại sau 1 phút.");
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
