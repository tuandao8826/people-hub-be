using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;

namespace Application.Modules.Subjects.Commands;

internal static class SubjectConstraintErrors
{
    public const string SubjectCodeIndex = "IX_subject_SubjectCode";
    public const string SubjectNameIndex = "IX_subject_Name";

    public static bool IsDuplicateCode(DbUpdateException exception)
        => exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: SubjectCodeIndex
        };

    public static bool IsDuplicateName(DbUpdateException exception)
        => exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: SubjectNameIndex
        };

    public static BadRequestException DuplicateCode(string code, Exception? innerException = null)
        => new($"Mã môn học '{code}' đã tồn tại trong hệ thống.", HttpStatusCode.Conflict, innerException);

    public static BadRequestException DuplicateName(string name, Exception? innerException = null)
        => new($"Tên môn học '{name}' đã tồn tại trong hệ thống.", HttpStatusCode.Conflict, innerException);

    public static BadRequestException InUse(Exception? innerException = null)
        => new("Không thể xóa môn học đang được sử dụng trong khóa học.", HttpStatusCode.Conflict, innerException);
}
