using System.Threading.RateLimiting;

namespace Application.Modules.Users.Commands.Login;

public sealed class LoginAttemptLimiter : IDisposable
{
    private readonly PartitionedRateLimiter<string> _limiter = PartitionedRateLimiter.Create<string, string>(
        username => RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(5),
            QueueLimit = 0,
        }));

    public bool TryAcquire(string username)
    {
        using var lease = _limiter.AttemptAcquire(username.Trim().ToUpperInvariant());
        return lease.IsAcquired;
    }

    public void Dispose() => _limiter.Dispose();
}
