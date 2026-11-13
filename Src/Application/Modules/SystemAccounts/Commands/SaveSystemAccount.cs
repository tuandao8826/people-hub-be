using System.Net;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.SystemAccounts.Bases;
using Application.Modules.Users.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.SystemAccounts.Commands;

public record SaveSystemAccountCommand(Guid? Id, UpdateSystemAccountRequest Data) : IRequest<SystemAccountResponse?>;

public class SaveSystemAccountHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SaveSystemAccountCommand, SystemAccountResponse?>
{
    public async Task<SystemAccountResponse?> Handle(SaveSystemAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == SystemAccountAccess.RootAdminId)
            throw new BadRequestException("SystemAccount.RootProtected", HttpStatusCode.Forbidden);

        var accounts = unitOfWork.Repository<User>();
        User? account;
        if (request.Id is Guid id)
        {
            account = await accounts.Find(user => user.Id == id && user.RoleId == SystemAccountAccess.AdministratorRoleId)
                .SingleOrDefaultAsync(cancellationToken);
            if (account is null) return null;
        }
        else
        {
            var username = ((CreateSystemAccountRequest)request.Data).Username.Trim();
            if (username.Length < 6 || string.IsNullOrEmpty(request.Data.Password))
                throw new BadRequestException("SystemAccount.CredentialsRequired");

            account = new User { Username = username, RoleId = SystemAccountAccess.AdministratorRoleId };
        }

        account.Name = request.Data.Name.Trim();
        account.Email = string.IsNullOrWhiteSpace(request.Data.Email) ? null : request.Data.Email.Trim();
        account.Status = request.Data.Status;
        if (request.Data.Password is { } password)
        {
            if (Encoding.UTF8.GetByteCount(password) > 72 || string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("SystemAccount.InvalidPassword");
            account.Password = PasswordExtension.Hash(password);
        }

        try
        {
            if (request.Id.HasValue) await accounts.UpdateAsync(account, cancellationToken);
            else await accounts.AddAsync(account, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_user_Username" })
        {
            throw new BadRequestException("SystemAccount.UsernameExists", HttpStatusCode.Conflict);
        }

        return new SystemAccountResponse(account.Id, account.Username, account.Name, account.Email, account.Status, false);
    }
}
