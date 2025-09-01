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

namespace Application.Modules.Users.Commands.Login;

public class LoginUserHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IOptions<SecuritySettings> securityOptions) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly JwtTokenProfiles jwtTokenProfiles = securityOptions.Value.JwtTokenProfiles;

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>()
            .Find(x => x.Username == request.Username)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<User>.Invalid());

        if (!PasswordExtension.Verify(request.Password!, user.Password))
        {
            throw new BadRequestException(Message<User>.Invalid());
        }

        if (user.Status == OperationStatus.Locked)
        {
            throw new BadRequestException(Message<User>.Blocked());
        }

        LoginUserResponse authToken = new()
        {
            AccessToken = jwtTokenService.GenerateAccessToken(jwtTokenProfiles.Admin, user),
            RefreshToken = jwtTokenService.GenerateRefreshToken(),
        };

        return authToken;
    }
}
