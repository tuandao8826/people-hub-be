using Application.Modules.Training.Queries;

namespace Api.TrainingNotifications;

public sealed class TrainingNotificationSnapshot
{
    private readonly object _gate = new();
    private Snapshot? _snapshot;
    private long _version;

    internal TrainingNotificationsResponse? GetToday()
    {
        var today = TrainingNotificationSchedule.GetLocalDate(DateTimeOffset.UtcNow);
        lock (_gate)
        {
            return _snapshot?.Date == today ? _snapshot.Notifications : null;
        }
    }

    internal long GetVersion()
    {
        lock (_gate)
        {
            return _version;
        }
    }

    internal bool TryPublish(DateOnly date, long version, TrainingNotificationsResponse notifications)
    {
        lock (_gate)
        {
            if (_version != version)
                return false;

            _snapshot = new Snapshot(date, notifications);
            return true;
        }
    }

    internal void Invalidate()
    {
        lock (_gate)
        {
            _version++;
            _snapshot = null;
        }
    }

    private sealed record Snapshot(DateOnly Date, TrainingNotificationsResponse Notifications);
}
