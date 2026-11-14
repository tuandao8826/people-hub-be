using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            if (!Regex.IsMatch(username, @"^[A-Za-z0-9._-]{6,50}$"))
                throw new BadRequestException("SystemAccount.InvalidUsername");

            account = new User { Username = username, RoleId = SystemAccountAccess.AdministratorRoleId };
        }

        var email = request.Data.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            throw new BadRequestException("SystemAccount.EmailRequired");
        var normalizedEmail = email.ToLowerInvariant();
        var duplicateEmailQuery = accounts.Find(user => user.Email != null && user.Email.ToLower() == normalizedEmail);
        if (request.Id is Guid currentId)
            duplicateEmailQuery = duplicateEmailQuery.Where(user => user.Id != currentId);
        var duplicateEmail = await duplicateEmailQuery.AnyAsync(cancellationToken);
        if (duplicateEmail)
            throw new BadRequestException("SystemAccount.EmailExists", HttpStatusCode.Conflict);

        account.Name = request.Data.Name.Trim();
        account.Email = email;
        account.Status = request.Data.Status;
        if (request.Data.Password is { } password)
        {
            if (Encoding.UTF8.GetByteCount(password) > 72 || string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("SystemAccount.InvalidPassword");
            if (password.Length is < 8 or > 50 ||
                !password.Any(char.IsUpper) || !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) || !password.Any(character => !char.IsLetterOrDigit(character)))
                throw new BadRequestException("SystemAccount.WeakPassword");
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
