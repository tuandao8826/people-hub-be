using System.Net;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using Application.Modules.Training.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Training.Commands;

public class ImportTrainingHandler(IUnitOfWork unitOfWork) : IRequestHandler<ImportTrainingCommand, ImportTrainingResult>
{
    public async Task<ImportTrainingResult> Handle(ImportTrainingCommand request, CancellationToken cancellationToken)
    {
        var employeeCodes = request.Employees.Select(item => item.Code.Trim().ToUpperInvariant()).ToArray();
        var subjectCodes = request.Subjects.Select(item => item.Code.Trim().ToUpperInvariant()).ToArray();
        var entries = request.Entries.Select(item => item with
        {
            StaffCode = item.StaffCode.Trim().ToUpperInvariant(),
            SubjectCode = item.SubjectCode.Trim().ToUpperInvariant()
        }).ToList();
        if (employeeCodes.Distinct().Count() != employeeCodes.Length ||
            subjectCodes.Distinct().Count() != subjectCodes.Length ||
            entries.Select(item => (item.StaffCode, item.SubjectCode)).Distinct().Count() != entries.Count)
            throw new BadRequestException("Tệp có mã hoặc hồ sơ đào tạo trùng.");

        var staffCodes = employeeCodes.ToHashSet();
        var courseCodes = subjectCodes.ToHashSet();
        if (entries.Any(item => item.TrainingDate.Year < 1900 ||
            !staffCodes.Contains(item.StaffCode) || !courseCodes.Contains(item.SubjectCode)))
            throw new BadRequestException("Hồ sơ đào tạo có ngày hoặc mã tham chiếu không hợp lệ.");

        var staff = unitOfWork.Repository<StaffMember>();
        var subjects = unitOfWork.Repository<Subject>();
        var records = unitOfWork.Repository<TrainingRecord>();
        var knownStaff = await staff.Find(item => employeeCodes.Contains(item.Code.ToUpper()))
            .AsNoTracking().ToDictionaryAsync(item => item.Code.ToUpperInvariant(), cancellationToken);
        var knownSubjects = await subjects.Find(item => subjectCodes.Contains(item.SubjectCode.ToUpper()))
            .AsNoTracking().ToDictionaryAsync(item => item.SubjectCode.ToUpperInvariant(), cancellationToken);

        var newStaff = new List<StaffMember>();
        foreach (var item in request.Employees)
        {
            var code = item.Code.Trim().ToUpperInvariant();
            if (knownStaff.TryGetValue(code, out var existing))
            {
                if (item.UsesTemporaryCode && !string.Equals(existing.Name.Trim(), item.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new BadRequestException($"Mã tạm {code} đã thuộc về nhân viên khác. Vui lòng điền Staff_Code cho dòng tương ứng.", HttpStatusCode.Conflict);
                continue;
            }
            var member = new StaffMember
            {
                Code = code, Name = item.Name.Trim(), Department = "Chưa cập nhật", JobTitle = "Chưa cập nhật"
            };
            knownStaff.Add(code, member);
            newStaff.Add(member);
        }

        var newSubjects = new List<Subject>();
        foreach (var item in request.Subjects)
        {
            var code = item.Code.Trim().ToUpperInvariant();
            if (knownSubjects.ContainsKey(code))
                continue;
            var subject = new Subject
            {
                SubjectCode = code, Name = item.Name.Trim(), CycleValue = item.CycleValue, SubjectCycleUnit = item.CycleUnit
            };
            knownSubjects.Add(code, subject);
            newSubjects.Add(subject);
        }

        var staffIds = knownStaff.Values.Select(item => item.Id).ToArray();
        var subjectIds = knownSubjects.Values.Select(item => item.Id).ToArray();
        var existingRecords = await records.Find(item => staffIds.Contains(item.StaffMemberId) && subjectIds.Contains(item.SubjectId))
            .AsNoTracking().ToListAsync(cancellationToken);
        var existingByPair = existingRecords.ToDictionary(item => (item.StaffMemberId, item.SubjectId));
        var newRecords = new List<TrainingRecord>();
        var recordsToUpdate = new List<TrainingRecord>();
        var unchangedRecords = 0;
        foreach (var item in entries)
        {
            var staffId = knownStaff[item.StaffCode].Id;
            var subject = knownSubjects[item.SubjectCode];
            var date = item.TrainingDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            try
            {
                _ = TrainingExpiry.AddCycle(date, subject);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new BadRequestException($"Ngày hết hạn vượt phạm vi cho {item.StaffCode} và {item.SubjectCode}.");
            }
            if (existingByPair.TryGetValue((staffId, subject.Id), out var existing))
            {
                if (existing.TrainingDate.UtcDateTime.Date == date.Date)
                {
                    unchangedRecords++;
                    continue;
                }
                existing.TrainingDate = date;
                recordsToUpdate.Add(existing);
            }
            else
            {
                newRecords.Add(new TrainingRecord { StaffMemberId = staffId, SubjectId = subject.Id, TrainingDate = date });
            }
        }

        var preview = new ImportTrainingResult(
            newStaff.Count, newSubjects.Count, newRecords.Count, recordsToUpdate.Count, unchangedRecords, 0);
        if (request.PreviewOnly) return preview;
        if (request.ExpectedResult is not null && request.ExpectedResult != preview)
            throw new BadRequestException("Dữ liệu đã thay đổi kể từ lần xem trước. Vui lòng xem lại số liệu trước khi đồng bộ.", HttpStatusCode.Conflict);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (newStaff.Count > 0) await staff.AddRangeAsync(newStaff, cancellationToken);
            if (newSubjects.Count > 0) await subjects.AddRangeAsync(newSubjects, cancellationToken);
            if (recordsToUpdate.Count > 0) await records.UpdateRangeAsync(recordsToUpdate, cancellationToken);
            if (newRecords.Count > 0) await records.AddRangeAsync(newRecords, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new BadRequestException("Dữ liệu đã thay đổi trong lúc nhập. Vui lòng thử lại.", HttpStatusCode.Conflict);
        }

        return preview;
    }
}
