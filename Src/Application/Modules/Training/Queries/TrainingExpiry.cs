using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;

namespace Application.Modules.Training.Queries;

internal static class TrainingExpiry
{
    private static readonly TimeZoneInfo BusinessTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

    // Training dates are stored as UTC calendar dates.
    internal static DateTimeOffset Today => new(
        TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, BusinessTimeZone).Date, TimeSpan.Zero);

    internal static DateTimeOffset AddCycle(DateTimeOffset date, Subject subject) => subject.SubjectCycleUnit switch
    {
        SubjectCycleUnit.Days => date.AddDays(subject.CycleValue),
        SubjectCycleUnit.Months => date.AddMonths(subject.CycleValue),
        SubjectCycleUnit.Years => date.AddYears(subject.CycleValue),
        _ => date
    };

    internal static IQueryable<TrainingExpiration> SelectExpirations(
        IQueryable<TrainingRecord> records, IQueryable<Subject> subjects)
        => from record in records
           join subject in subjects on record.SubjectId equals subject.Id
           select new TrainingExpiration
           {
               StaffMemberId = record.StaffMemberId,
               ExpirationDate = subject.SubjectCycleUnit == SubjectCycleUnit.Days ? record.TrainingDate.AddDays(subject.CycleValue) :
                   subject.SubjectCycleUnit == SubjectCycleUnit.Months ? record.TrainingDate.AddMonths(subject.CycleValue) :
                   subject.SubjectCycleUnit == SubjectCycleUnit.Years ? record.TrainingDate.AddYears(subject.CycleValue) :
                   record.TrainingDate
           };

    internal static IQueryable<TrainingExpiration> Filter(
        this IQueryable<TrainingExpiration> records, TrainingRecordStatus status, DateTimeOffset today)
    {
        var tomorrow = today.AddDays(1);
        var warningLimit = today.AddDays(15);
        return status switch
        {
            TrainingRecordStatus.Expired => records.Where(record => record.ExpirationDate < today),
            TrainingRecordStatus.ExpiresToday => records.Where(record => record.ExpirationDate >= today && record.ExpirationDate < tomorrow),
            TrainingRecordStatus.ExpiringSoon => records.Where(record => record.ExpirationDate >= tomorrow && record.ExpirationDate < warningLimit),
            TrainingRecordStatus.Valid => records.Where(record => record.ExpirationDate >= warningLimit),
            _ => records.Where(_ => false)
        };
    }
}

internal sealed class TrainingExpiration
{
    public Guid StaffMemberId { get; init; }
    public DateTimeOffset ExpirationDate { get; init; }
}
