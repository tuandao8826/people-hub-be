using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using ClosedXML.Excel;
using Core.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public sealed record ExportExpiredTrainingQuery(Guid? SubjectId = null) : IRequest<TrainingMatrixExportFile>;

public sealed class ExportExpiredTrainingHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ExportExpiredTrainingQuery, TrainingMatrixExportFile>
{
    private sealed record ExportSubject(Guid Id, string Code, string Name, int CycleValue, SubjectCycleUnit CycleUnit);
    private sealed record ExportEmployee(Guid Id, string Code, string Name);
    private sealed record ExportRecord(Guid StaffId, Guid SubjectId, DateTimeOffset TrainingDate);

    public async Task<TrainingMatrixExportFile> Handle(
        ExportExpiredTrainingQuery request, CancellationToken cancellationToken)
    {
        var activeStaffQuery = unitOfWork.Repository<StaffMember>().Find(x => x.Status == StaffStatus.Active).AsNoTracking();
        var activeSubjectQuery = unitOfWork.Repository<Subject>().Find(x =>
            x.OperationStatus == OperationStatus.Active && (!request.SubjectId.HasValue || x.Id == request.SubjectId.Value))
            .AsNoTracking();
        var employees = await activeStaffQuery.OrderBy(x => x.Code)
            .Select(x => new ExportEmployee(x.Id, x.Code, x.Name)).ToListAsync(cancellationToken);
        var subjects = await activeSubjectQuery.OrderBy(x => x.Name)
            .Select(x => new ExportSubject(x.Id, x.SubjectCode, x.Name, x.CycleValue, x.SubjectCycleUnit))
            .ToListAsync(cancellationToken);
        var records = await unitOfWork.Repository<TrainingRecord>().Find()
            .Where(record => activeStaffQuery.Any(staff => staff.Id == record.StaffMemberId) &&
                             activeSubjectQuery.Any(subject => subject.Id == record.SubjectId))
            .AsNoTracking().Select(x => new ExportRecord(x.StaffMemberId, x.SubjectId, x.TrainingDate))
            .ToListAsync(cancellationToken);

        var recordByPair = records.ToDictionary(x => (x.StaffId, x.SubjectId));
        var today = DateTime.UtcNow.AddHours(7).Date;
        using var workbook = new XLWorkbook();
        var usedSheetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var subject in subjects)
        {
            var worksheet = workbook.Worksheets.Add(UniqueSheetName(subject.Name, usedSheetNames));
            worksheet.Range("A1:E1").Merge();
            worksheet.Cell("A1").Value = $"MÔN HỌC: {subject.Name}     ·     NGÀY XUẤT: {today:dd/MM/yyyy}";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 14;
            worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;
            worksheet.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#24476B");
            worksheet.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Row(1).Height = 32;
            worksheet.Row(2).Height = 9;

            var headers = new[] { "STT", "Mã nhân viên", "Tên nhân viên", "Ngày học", "Ngày hết hạn" };
            for (var column = 1; column <= headers.Length; column++)
            {
                var cell = worksheet.Cell(3, column);
                cell.Value = headers[column - 1];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3978A8");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                SetBorder(cell);
            }
            worksheet.Row(3).Height = 28;

            var sheetRow = 4;
            foreach (var employee in employees)
            {
                if (!recordByPair.TryGetValue((employee.Id, subject.Id), out var record)) continue;
                var trainingDate = record.TrainingDate.UtcDateTime.Date;
                var expirationDate = subject.CycleUnit switch
                {
                    SubjectCycleUnit.Days => trainingDate.AddDays(subject.CycleValue),
                    SubjectCycleUnit.Months => trainingDate.AddMonths(subject.CycleValue),
                    SubjectCycleUnit.Years => trainingDate.AddYears(subject.CycleValue),
                    _ => trainingDate
                };
                if (expirationDate >= today) continue;

                var rowValues = new object[] { sheetRow - 3, employee.Code, employee.Name, trainingDate, expirationDate };
                for (var column = 1; column <= rowValues.Length; column++)
                {
                    var cell = worksheet.Cell(sheetRow, column);
                    cell.Value = XLCellValue.FromObject(rowValues[column - 1]);
                    cell.Style.Font.FontName = "Times New Roman";
                    cell.Style.Font.FontSize = 11;
                    cell.Style.Fill.BackgroundColor = sheetRow % 2 == 0
                        ? XLColor.White : XLColor.FromHtml("#F3F6FA");
                    cell.Style.Alignment.Horizontal = column == 3 ? XLAlignmentHorizontalValues.Left : XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    if (column >= 4)
                    {
                        cell.Style.NumberFormat.Format = "dd/mm/yyyy";
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCE4E4");
                    }
                    SetBorder(cell);
                }
                worksheet.Row(sheetRow).Height = 23;
                sheetRow++;
            }

            worksheet.Column(1).Width = 9;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 34;
            worksheet.Column(4).Width = 18;
            worksheet.Column(5).Width = 18;
            worksheet.SheetView.FreezeRows(3);
            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            worksheet.PageSetup.FitToPages(1, 0);
            worksheet.PageSetup.PagesWide = 1;
            worksheet.PageSetup.SetRowsToRepeatAtTop(1, 3);
            worksheet.Range(3, 1, Math.Max(3, sheetRow - 1), 5).SetAutoFilter();
        }

        if (subjects.Count == 0)
        {
            var worksheet = workbook.Worksheets.Add("Nhan vien het han");
            worksheet.Cell("A1").Value = "Không có môn học đang hoạt động phù hợp để xuất.";
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new TrainingMatrixExportFile(stream.ToArray(), $"Nhan_vien_da_het_han_{today:yyyyMMdd}.xlsx");
    }

    private static string UniqueSheetName(string subjectName, HashSet<string> usedNames)
    {
        var baseName = new string(subjectName.Select(character => "[]:*?/\\".Contains(character) ? ' ' : character).ToArray())
            .Trim();
        if (string.IsNullOrWhiteSpace(baseName)) baseName = "Mon hoc";
        baseName = baseName[..Math.Min(baseName.Length, 31)];
        var name = baseName;
        var suffix = 2;
        while (!usedNames.Add(name))
        {
            var ending = $" ({suffix++})";
            name = $"{baseName[..Math.Min(baseName.Length, 31 - ending.Length)]}{ending}";
        }
        return name;
    }

    private static void SetBorder(IXLCell cell)
    {
        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B7C1CD");
    }
}
