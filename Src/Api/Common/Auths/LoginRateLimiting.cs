using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Api.Common.Auths;

internal static class LoginRateLimiting
{
    internal const string Policy = "LoginByIp";

    internal static IServiceCollection AddLoginRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(Policy, context =>
            {
                var address = context.Connection.RemoteIpAddress;
                if (address?.IsIPv4MappedToIPv6 == true)
                    address = address.MapToIPv4();

                return RateLimitPartition.GetFixedWindowLimiter(address?.ToString() ?? "unknown", _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0,
                    });
            });
        });
    }
}
