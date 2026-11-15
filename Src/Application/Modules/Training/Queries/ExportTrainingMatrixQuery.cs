using Application.Common.Interfaces.Persistence;
using Application.Modules.Staff.Entities;
using Application.Modules.Subjects.Entites;
using Application.Modules.Training.Entities;
using ClosedXML.Excel;
using Core.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Training.Queries;

public sealed record ExportTrainingMatrixQuery : IRequest<TrainingMatrixExportFile>;

public sealed record TrainingMatrixExportFile(byte[] Content, string FileName);

public sealed class ExportTrainingMatrixHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ExportTrainingMatrixQuery, TrainingMatrixExportFile>
{
    private sealed record ExportSubject(Guid Id, string SubjectCode, string Name, int CycleValue, SubjectCycleUnit SubjectCycleUnit);
    private sealed record ExportRecord(Guid StaffMemberId, Guid SubjectId, DateTimeOffset TrainingDate);

    public async Task<TrainingMatrixExportFile> Handle(
        ExportTrainingMatrixQuery request, CancellationToken cancellationToken)
    {
        var activeStaffQuery = unitOfWork.Repository<StaffMember>().Find(x => x.Status == StaffStatus.Active).AsNoTracking();
        var activeSubjectQuery = unitOfWork.Repository<Subject>()
            .Find(x => x.OperationStatus == OperationStatus.Active).AsNoTracking();
        var employees = await activeStaffQuery.OrderBy(x => x.Code)
            .Select(x => new { x.Id, x.Code, x.Name })
            .ToListAsync(cancellationToken);
        var subjects = await activeSubjectQuery.OrderBy(x => x.Name)
            .Select(x => new ExportSubject(x.Id, x.SubjectCode, x.Name, x.CycleValue, x.SubjectCycleUnit))
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Quản lý đào tạo");
        var lastColumn = 4 + subjects.Count * 2;
        var lastColumnLetter = XLHelper.GetColumnLetterFromNumber(lastColumn);
        var today = DateTime.UtcNow.AddHours(7).Date;

        worksheet.Range(1, 1, 1, lastColumn).Merge();
        worksheet.Cell(1, 1).Value = "PEOPLEHUB · HR & TRAINING";
        worksheet.Range(2, 1, 2, lastColumn).Merge();
        worksheet.Cell(2, 1).Value = "QUẢN LÝ ĐÀO TẠO";
        worksheet.Range(3, 1, 3, lastColumn).Merge();
        worksheet.Cell(3, 1).Value = $"BẢNG TỔNG HỢP ĐÀO TẠO · NGÀY XUẤT {today:dd/MM/yyyy}";
        for (var row = 1; row <= 3; row++)
        {
            var cell = worksheet.Cell(row, 1);
            var titleRange = worksheet.Range(row, 1, row, lastColumn);
            titleRange.Style.Font.FontName = "Times New Roman";
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = row == 3 ? 12 : 14;
            titleRange.Style.Font.FontColor = XLColor.White;
            titleRange.Style.Fill.BackgroundColor = row == 3 ? XLColor.FromHtml("#376A9A") : XLColor.FromHtml("#24476B");
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.Horizontal = row == 3 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
            worksheet.Row(row).Height = row == 1 ? 28 : row == 2 ? 25 : 29;
        }

        var fixedHeaders = new[] { "STT", "Staff_Code", "Staff_Name", "Birthday" };
        for (var column = 1; column <= fixedHeaders.Length; column++)
            worksheet.Cell(4, column).Value = fixedHeaders[column - 1];

        for (var index = 0; index < subjects.Count; index++)
        {
            var firstColumn = 5 + index * 2;
            var subject = subjects[index];
            worksheet.Cell(4, firstColumn).Value = subject.SubjectCode;
            worksheet.Cell(4, firstColumn + 1).Value = subject.SubjectCode;
            worksheet.Range(5, firstColumn, 5, firstColumn + 1).Merge();
            worksheet.Cell(5, firstColumn).Value = $"{subject.Name}\n({subject.CycleValue} {CycleUnitLabel(subject.SubjectCycleUnit)})";
            worksheet.Cell(6, firstColumn).Value = "Ngày học";
            worksheet.Cell(6, firstColumn + 1).Value = "Ngày hết hạn";
        }

        var humanHeaders = new[] { "STT", "Mã nhân viên", "Họ và tên", "Ngày, tháng\nnăm sinh" };
        for (var column = 1; column <= humanHeaders.Length; column++)
        {
            worksheet.Range(5, column, 6, column).Merge();
            worksheet.Cell(5, column).Value = humanHeaders[column - 1];
        }

        for (var row = 4; row <= 6; row++)
        {
            worksheet.Row(row).Height = row == 4 ? 25 : row == 5 ? 62 : 34;
            for (var column = 1; column <= lastColumn; column++)
            {
                var cell = worksheet.Cell(row, column);
                cell.Style.Font.FontName = "Times New Roman";
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = row == 5 ? 11 : 10;
                cell.Style.Font.FontColor = XLColor.FromHtml("#172554");
                cell.Style.Fill.BackgroundColor = row == 6 && column > 4 && column % 2 == 0
                    ? XLColor.FromHtml("#FFF2A6")
                    : row == 4 ? XLColor.FromHtml("#D9E2F3") : XLColor.FromHtml("#EAF0F8");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                SetBorder(cell);
            }
        }

        worksheet.Column(1).Width = 8;
        worksheet.Column(2).Width = 17;
        worksheet.Column(3).Width = 27;
        worksheet.Column(4).Width = 16;
        for (var index = 0; index < subjects.Count; index++)
        {
            worksheet.Column(5 + index * 2).Width = 15;
            worksheet.Column(6 + index * 2).Width = 15;
        }

        var records = await unitOfWork.Repository<TrainingRecord>().Find()
                .Where(record => activeStaffQuery.Any(staff => staff.Id == record.StaffMemberId) &&
                                 activeSubjectQuery.Any(subject => subject.Id == record.SubjectId))
                .AsNoTracking().Select(x => new ExportRecord(x.StaffMemberId, x.SubjectId, x.TrainingDate))
                .ToListAsync(cancellationToken);
        var recordMap = records.ToDictionary(x => (x.StaffMemberId, x.SubjectId));

        for (var index = 0; index < employees.Count; index++)
        {
            var employee = employees[index];
            var rowNumber = 7 + index;
            worksheet.Row(rowNumber).Height = 23;
            var baseFill = index % 2 == 0 ? XLColor.White : XLColor.FromHtml("#F3F6FA");
            var fixedValues = new object?[] { index + 1, employee.Code, employee.Name, null };
            for (var column = 1; column <= fixedValues.Length; column++)
            {
                var cell = worksheet.Cell(rowNumber, column);
                if (fixedValues[column - 1] is not null) cell.Value = XLCellValue.FromObject(fixedValues[column - 1]);
                cell.Style.Fill.BackgroundColor = baseFill;
                cell.Style.Font.FontName = "Times New Roman";
                cell.Style.Font.FontSize = 11;
                cell.Style.Font.Bold = column == 2;
                cell.Style.Alignment.Horizontal = column == 3 ? XLAlignmentHorizontalValues.Left : XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SetBorder(cell);
            }

            for (var subjectIndex = 0; subjectIndex < subjects.Count; subjectIndex++)
            {
                var subject = subjects[subjectIndex];
                var trainingCell = worksheet.Cell(rowNumber, 5 + subjectIndex * 2);
                var expiryCell = worksheet.Cell(rowNumber, 6 + subjectIndex * 2);
                foreach (var cell in new[] { trainingCell, expiryCell })
                {
                    cell.Style.Fill.BackgroundColor = baseFill;
                    cell.Style.Font.FontName = "Times New Roman";
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.NumberFormat.Format = "dd/mm/yyyy";
                    SetBorder(cell);
                }

                if (!recordMap.TryGetValue((employee.Id, subject.Id), out var record)) continue;
                var trainingDate = record.TrainingDate.UtcDateTime.Date;
                var expirationDate = subject.SubjectCycleUnit switch
                {
                    SubjectCycleUnit.Days => trainingDate.AddDays(subject.CycleValue),
                    SubjectCycleUnit.Months => trainingDate.AddMonths(subject.CycleValue),
                    SubjectCycleUnit.Years => trainingDate.AddYears(subject.CycleValue),
                    _ => trainingDate
                };
                trainingCell.Value = trainingDate;
                expiryCell.Value = expirationDate;
                var remainingDays = (expirationDate - today).Days;
                expiryCell.Style.Fill.BackgroundColor = remainingDays < 0
                    ? XLColor.FromHtml("#FCE4E4")
                    : remainingDays == 0
                        ? XLColor.FromHtml("#FCE4D6")
                        : remainingDays <= 14
                            ? XLColor.FromHtml("#FFF2CC")
                            : XLColor.FromHtml("#E2F0D9");
            }
        }

        var lastRow = Math.Max(6, 6 + employees.Count);
        worksheet.SheetView.Freeze(6, 4);
        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        worksheet.PageSetup.FitToPages(1, 0);
        worksheet.PageSetup.PagesWide = 1;
        worksheet.PageSetup.PrintAreas.Add($"A1:{lastColumnLetter}{lastRow}");
        worksheet.PageSetup.SetRowsToRepeatAtTop(1, 6);
        worksheet.Style.Font.FontName = "Times New Roman";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new TrainingMatrixExportFile(stream.ToArray(), $"Bang_quan_ly_dao_tao_{today:yyyyMMdd}.xlsx");
    }

    private static string CycleUnitLabel(SubjectCycleUnit unit) => unit switch
    {
        SubjectCycleUnit.Days => "Ngày",
        SubjectCycleUnit.Months => "Tháng",
        SubjectCycleUnit.Years => "Năm",
        _ => "Ngày"
    };

    private static void SetBorder(IXLCell cell)
    {
        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B7C1CD");
    }
}
