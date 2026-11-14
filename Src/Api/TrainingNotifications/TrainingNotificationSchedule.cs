namespace Api.TrainingNotifications;

internal sealed class TrainingNotificationSchedule(TimeOnly dailyRunAt)
{
    private static readonly TimeZoneInfo BusinessTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

    internal static DateOnly GetLocalDate(DateTimeOffset instant) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(instant, BusinessTimeZone).DateTime);

    internal DateTimeOffset GetNextRun(DateTimeOffset now)
    {
        var date = GetLocalDate(now);
        var scheduledAt = AtScheduledTime(date);
        return scheduledAt > now ? scheduledAt : AtScheduledTime(date.AddDays(1));
    }

    private DateTimeOffset AtScheduledTime(DateOnly date)
    {
        var localTime = date.ToDateTime(dailyRunAt);
        return new DateTimeOffset(localTime, BusinessTimeZone.GetUtcOffset(localTime));
    }
}
