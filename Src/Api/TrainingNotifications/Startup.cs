using System.Globalization;

namespace Api.TrainingNotifications;

internal static class Startup
{
    internal static IServiceCollection AddTrainingNotifications(
        this IServiceCollection services, IConfiguration configuration)
    {
        const string scheduleKey = "TrainingNotifications:DailyRunAt";
        var configuredTime = configuration[scheduleKey] ?? "00:30";
        if (!TimeOnly.TryParseExact(configuredTime, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dailyRunAt))
        {
            throw new InvalidOperationException($"{scheduleKey} must be a time in HH:mm format.");
        }

        services.AddSingleton(new TrainingNotificationSchedule(dailyRunAt));
        services.AddHostedService<TrainingNotificationJob>();
        return services;
    }
}
