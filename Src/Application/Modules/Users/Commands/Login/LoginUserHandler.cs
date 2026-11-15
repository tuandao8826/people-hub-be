using Application.Common.Auths.Configurations;
using Application.Common.Auths.Jwt;
using Application.Common.Auths.Services;
using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Users.Entities;
using Core.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace Application.Modules.Users.Commands.Login;

public class LoginUserHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IOptions<SecuritySettings> securityOptions,
    LoginAttemptLimiter attempts) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly JwtTokenProfiles jwtTokenProfiles = securityOptions.Value.JwtTokenProfiles;

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if (!attempts.TryAcquire(request.Username!))
            throw new BadRequestException("Bạn đã thử đăng nhập quá nhiều lần. Vui lòng thử lại sau.", HttpStatusCode.TooManyRequests);

        User user = await unitOfWork.Repository<User>()
            .Find(x => x.Username == request.Username)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<User>.Invalid());

        if (!PasswordExtension.Verify(request.Password!, user.Password))
        {
            throw new BadRequestException(Message<User>.Invalid());
        }

        if (user.Status != OperationStatus.Active)
        {
            throw new BadRequestException(Message<User>.Blocked());
        }

        var now = DateTimeOffset.UtcNow;
        var session = new LoginSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpiresAt = now.AddMinutes(jwtTokenProfiles.Admin.TokenValidityInMinutes)
        };
        var sessions = unitOfWork.Repository<LoginSession>();
        await sessions.Find(item => item.ExpiresAt <= now).ExecuteDeleteAsync(cancellationToken);

        var authToken = new LoginUserResponse
        {
            AccessToken = jwtTokenService.GenerateAccessToken(jwtTokenProfiles.Admin, user, session),
            RefreshToken = jwtTokenService.GenerateRefreshToken(),
        };

        await sessions.AddAsync(session, cancellationToken);
        return authToken;
    }
}
