using Application.Modules.Staff.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Staff.Commands;

public class DuplicateStaffCodeException(Exception innerException)
    : Exception("The staff code is already in use.", innerException)
{
    internal static bool IsDuplicateCode(DbUpdateException exception)
        => exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: StaffMemberConfiguration.CodeIndex
        };
}
