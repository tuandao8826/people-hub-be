using System.Globalization;
using Application.Common.Interfaces.Persistence;
using Application.Modules.FlightSchedules.Contracts;
using Application.Modules.FlightSchedules.Entities;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace Application.Modules.FlightSchedules.Commands;

public record ImportFlightSchedulesCommand(string FileName, byte[] Content, bool PreviewOnly = false) : IRequest<FlightImportSummary>;

public class ImportFlightSchedulesHandler(IUnitOfWork unitOfWork) : IRequestHandler<ImportFlightSchedulesCommand, FlightImportSummary>
{
    public async Task<FlightImportSummary> Handle(ImportFlightSchedulesCommand request, CancellationToken cancellationToken)
    {
        var rows = ReadRows(request.FileName, request.Content);
        if (rows.Count == 0) throw new ArgumentException("File không có dòng lịch bay hợp lệ.");
        var duplicate = rows.GroupBy(x => (x.FlightNumber.ToUpperInvariant(), x.FlightDate, x.Route.ToUpperInvariant())).FirstOrDefault(x => x.Count() > 1);
        if (duplicate is not null) throw new ArgumentException($"File có chuyến bay trùng mã, ngày và tuyến: {duplicate.Key.Item1} - {duplicate.Key.FlightDate:dd/MM/yyyy} - {duplicate.Key.Item3}.");

        var repo = unitOfWork.Repository<FlightSchedule>();
        var codes = rows.Select(x => x.FlightNumber.ToUpper()).Distinct().ToArray();
        var minDate = rows.Min(x => x.FlightDate); var maxDate = rows.Max(x => x.FlightDate);
        var existing = await repo.Find(x => codes.Contains(x.FlightNumber) && x.FlightDate >= minDate && x.FlightDate <= maxDate).ToListAsync(cancellationToken);
        var lookup = existing.ToDictionary(x => (x.FlightNumber.ToUpperInvariant(), x.FlightDate, x.Route.ToUpperInvariant()));
        var created = 0; var updated = 0; var unchanged = 0;
        var additions = new List<FlightSchedule>(); var changes = new List<FlightSchedule>();
        foreach (var row in rows)
        {
            if (!lookup.TryGetValue((row.FlightNumber.ToUpperInvariant(), row.FlightDate, row.Route.ToUpperInvariant()), out var entity))
            {
                entity = new FlightSchedule(); Apply(entity, row); additions.Add(entity); created++;
            }
            else
            {
                if (Equivalent(entity, row)) { unchanged++; continue; }
                Apply(entity, row); changes.Add(entity); updated++;
            }
        }
        if (!request.PreviewOnly)
        {
            if (additions.Count > 0) await repo.AddRangeAsync(additions, cancellationToken);
            if (changes.Count > 0) await repo.UpdateRangeAsync(changes, cancellationToken);
        }
        return new FlightImportSummary(created, updated, unchanged, 0, rows.Count, Array.Empty<string>());
    }

    private static List<FlightSchedule> ReadRows(string fileName, byte[] content)
    {
        var raw = new List<string[]>();
        if (Path.GetExtension(fileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = new MemoryStream(content); using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheets.First();
            foreach (var row in sheet.RowsUsed()) raw.Add(row.Cells(1, sheet.LastColumnUsed().ColumnNumber()).Select(c => c.GetString()).ToArray());
        }
        else
        {
            using var reader = new StringReader(System.Text.Encoding.UTF8.GetString(content).TrimStart('\uFEFF'));
            using var parser = new TextFieldParser(reader) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = false };
            parser.SetDelimiters(",", ";", "\t");
            while (!parser.EndOfData) raw.Add(parser.ReadFields() ?? []);
        }
        if (raw.Count < 2) return [];
        var headerRow = raw.FindIndex(r => r.Any(c => c.Trim().Equals("FlightNo", StringComparison.OrdinalIgnoreCase)));
        if (headerRow < 0) throw new ArgumentException("Không tìm thấy dòng tiêu đề có cột FlightNo.");
        var headers = raw[headerRow].Select((value, index) => (Name: value.Trim(), Index: index)).Where(x => x.Name.Length > 0).ToDictionary(x => x.Name, x => x.Index, StringComparer.OrdinalIgnoreCase);
        foreach (var required in new[] { "FlightNo", "FlightDate", "Route" }) if (!headers.ContainsKey(required)) throw new ArgumentException($"Thiếu cột bắt buộc {required}.");
        string? Get(string[] row, string key) => headers.TryGetValue(key, out var i) && i < row.Length ? Empty(row[i]) : null;
        var result = new List<FlightSchedule>();
        for (var i = headerRow + 1; i < raw.Count; i++)
        {
            var row = raw[i]; var flight = Get(row, "FlightNo"); if (string.IsNullOrWhiteSpace(flight)) continue;
            var dateText = Get(row, "FlightDate");
            if (!DateOnly.TryParseExact(dateText, ["ddMMMyyyy", "dd-MMM-yyyy", "d-MMM-yy", "dd-MMM-yy", "yyyy-MM-dd", "M/d/yyyy", "dd/MM/yyyy"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                throw new ArgumentException($"Dòng {i + 1}: FlightDate không hợp lệ ({dateText}).");
            result.Add(new FlightSchedule {
                FlightNumber = flight.ToUpperInvariant(), FlightDate = date, Route = (Get(row, "Route") ?? "").ToUpperInvariant(),
                IcaoFlightNumber = Get(row, "FlightNoIcao"), RegistrationNumber = Get(row, "AcregNo")?.ToUpperInvariant(), AircraftType = Get(row, "Actype"), FlightStatus = Get(row, "Fltst"),
                ArrivalFlightNumber = Get(row, "AflightNo"), DepartureFlightNumber = Get(row, "DflightNo"),
                ScheduledArrivalAt = ReadTime(Get(row, "Sibt")), ScheduledDepartureAt = ReadTime(Get(row, "Sobt")), EstimatedDepartureAt = ReadTime(Get(row, "Eobt")),
                ActualLandingAt = ReadTime(Get(row, "Aldt")), ActualTakeoffAt = ReadTime(Get(row, "Atot")), ArrivalStand = Get(row, "Aprk"), DepartureStand = Get(row, "Dprk"), Belt = Get(row, "Belt"),
                ArrivalFlightDate = ReadDateOnly(Get(row, "AflightDate")), DepartureFlightDate = ReadDateOnly(Get(row, "DflightDate")),
                ArrivalRouteIcao = Get(row, "ArouteIcao"), DepartureRouteIcao = Get(row, "DrouteIcao"), HandlingAgent = Get(row, "Mgha"),
                LandingRunway = Get(row, "Lrwy"), TakeoffRunway = Get(row, "Trwy"), NatureOfFlight = Get(row, "NatureOfFlight"),
                EstimatedInBlockAt = ReadTime(Get(row, "Eibt")), EstimatedTaxiTime = Get(row, "Ettt"), TargetOffBlockAt = ReadTime(Get(row, "Tobt")),
                CalculatedTakeoffAt = ReadTime(Get(row, "Ctot")), TargetStartupApprovalAt = ReadTime(Get(row, "Tsat")), ActualStartupApprovalAt = ReadTime(Get(row, "Asbt")),
                ActualGroundHandlingAt = ReadTime(Get(row, "Acgt")), ActualOffBlockAt = ReadTime(Get(row, "Aobt")), ActualInBlockAt = ReadTime(Get(row, "Aibt")),
                ActualRunwayDepartureAt = ReadTime(Get(row, "Ardt")), ActualEngineStartAt = ReadTime(Get(row, "Aegt")), ActualStartupAt = ReadTime(Get(row, "Asat")),
                TargetTaxiTime = ReadTime(Get(row, "Ttot")), ArrivalGate = Get(row, "Agate"), DepartureGate = Get(row, "Dgate"),
                EstimatedLandingAt = ReadTime(Get(row, "Eldt")), ActualStartRequestAt = ReadTime(Get(row, "Asrt")), SourceRowIndex = Get(row, "RowIndex"),
                ArrivalIcaoFlightNumber = Get(row, "AflightNoIcao"), DepartureIcaoFlightNumber = Get(row, "DflightNoIcao"),
                EstimatedDepartureAtSource = ReadTime(Get(row, "EOBT1")), EstimatedArrivalAt = ReadTime(Get(row, "ETA")),
                TargetOffBlockCount = Get(row, "Tobt_Count"), TargetStartupApprovalCount = Get(row, "Tsat_Count")
            });
            if (string.IsNullOrWhiteSpace(result[^1].Route)) throw new ArgumentException($"Dòng {i + 1}: Route không được để trống.");
        }
        return result;
    }
    private static string? Empty(string? value) => string.IsNullOrWhiteSpace(value) || value.Trim() is "---" or "-" ? null : value.Trim();
    private static DateOnly? ReadDateOnly(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (DateOnly.TryParseExact(value, ["yyyy-MM-dd", "M/d/yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "ddMMMyyyy"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact)) return exact;
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)) return DateOnly.FromDateTime(parsed);
        throw new ArgumentException($"Giá trị ngày không hợp lệ: {value}.");
    }
    private static DateTimeOffset? ReadTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if ((trimmed.EndsWith('Z') || System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"[+-]\d{2}:?\d{2}$")) &&
            DateTimeOffset.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var explicitOffset)) return explicitOffset.ToUniversalTime();
        if (DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), TimeSpan.FromHours(7)).ToUniversalTime();
        throw new ArgumentException($"Giá trị thời gian không hợp lệ: {value}.");
    }
    private static void Apply(FlightSchedule e, FlightSchedule s)
    {
        e.FlightNumber=s.FlightNumber; e.FlightDate=s.FlightDate; e.Route=s.Route; e.IcaoFlightNumber=s.IcaoFlightNumber; e.RegistrationNumber=s.RegistrationNumber; e.AircraftType=s.AircraftType; e.FlightStatus=s.FlightStatus; e.ArrivalFlightNumber=s.ArrivalFlightNumber; e.DepartureFlightNumber=s.DepartureFlightNumber; e.ScheduledArrivalAt=s.ScheduledArrivalAt; e.ScheduledDepartureAt=s.ScheduledDepartureAt; e.EstimatedDepartureAt=s.EstimatedDepartureAt; e.ActualLandingAt=s.ActualLandingAt; e.ActualTakeoffAt=s.ActualTakeoffAt; e.ArrivalStand=s.ArrivalStand; e.DepartureStand=s.DepartureStand; e.Belt=s.Belt;
        e.ArrivalFlightDate=s.ArrivalFlightDate; e.DepartureFlightDate=s.DepartureFlightDate; e.ArrivalRouteIcao=s.ArrivalRouteIcao; e.DepartureRouteIcao=s.DepartureRouteIcao; e.HandlingAgent=s.HandlingAgent; e.LandingRunway=s.LandingRunway; e.TakeoffRunway=s.TakeoffRunway; e.NatureOfFlight=s.NatureOfFlight; e.EstimatedInBlockAt=s.EstimatedInBlockAt; e.EstimatedTaxiTime=s.EstimatedTaxiTime; e.TargetOffBlockAt=s.TargetOffBlockAt; e.CalculatedTakeoffAt=s.CalculatedTakeoffAt; e.TargetStartupApprovalAt=s.TargetStartupApprovalAt; e.ActualStartupApprovalAt=s.ActualStartupApprovalAt; e.ActualGroundHandlingAt=s.ActualGroundHandlingAt; e.ActualOffBlockAt=s.ActualOffBlockAt; e.ActualInBlockAt=s.ActualInBlockAt; e.ActualRunwayDepartureAt=s.ActualRunwayDepartureAt; e.ActualEngineStartAt=s.ActualEngineStartAt; e.ActualStartupAt=s.ActualStartupAt; e.TargetTaxiTime=s.TargetTaxiTime; e.ArrivalGate=s.ArrivalGate; e.DepartureGate=s.DepartureGate; e.EstimatedLandingAt=s.EstimatedLandingAt; e.ActualStartRequestAt=s.ActualStartRequestAt; e.SourceRowIndex=s.SourceRowIndex; e.ArrivalIcaoFlightNumber=s.ArrivalIcaoFlightNumber; e.DepartureIcaoFlightNumber=s.DepartureIcaoFlightNumber; e.EstimatedDepartureAtSource=s.EstimatedDepartureAtSource; e.EstimatedArrivalAt=s.EstimatedArrivalAt; e.TargetOffBlockCount=s.TargetOffBlockCount; e.TargetStartupApprovalCount=s.TargetStartupApprovalCount;
    }
    private static bool Equivalent(FlightSchedule a, FlightSchedule b) => a.Route==b.Route && a.IcaoFlightNumber==b.IcaoFlightNumber && a.RegistrationNumber==b.RegistrationNumber && a.AircraftType==b.AircraftType && a.FlightStatus==b.FlightStatus && a.ArrivalFlightNumber==b.ArrivalFlightNumber && a.DepartureFlightNumber==b.DepartureFlightNumber && a.ScheduledArrivalAt==b.ScheduledArrivalAt && a.ScheduledDepartureAt==b.ScheduledDepartureAt && a.EstimatedDepartureAt==b.EstimatedDepartureAt && a.ActualLandingAt==b.ActualLandingAt && a.ActualTakeoffAt==b.ActualTakeoffAt && a.ArrivalStand==b.ArrivalStand && a.DepartureStand==b.DepartureStand && a.Belt==b.Belt && a.ArrivalFlightDate==b.ArrivalFlightDate && a.DepartureFlightDate==b.DepartureFlightDate && a.ArrivalRouteIcao==b.ArrivalRouteIcao && a.DepartureRouteIcao==b.DepartureRouteIcao && a.HandlingAgent==b.HandlingAgent && a.LandingRunway==b.LandingRunway && a.TakeoffRunway==b.TakeoffRunway && a.NatureOfFlight==b.NatureOfFlight && a.EstimatedInBlockAt==b.EstimatedInBlockAt && a.EstimatedTaxiTime==b.EstimatedTaxiTime && a.TargetOffBlockAt==b.TargetOffBlockAt && a.CalculatedTakeoffAt==b.CalculatedTakeoffAt && a.TargetStartupApprovalAt==b.TargetStartupApprovalAt && a.ActualStartupApprovalAt==b.ActualStartupApprovalAt && a.ActualGroundHandlingAt==b.ActualGroundHandlingAt && a.ActualOffBlockAt==b.ActualOffBlockAt && a.ActualInBlockAt==b.ActualInBlockAt && a.ActualRunwayDepartureAt==b.ActualRunwayDepartureAt && a.ActualEngineStartAt==b.ActualEngineStartAt && a.ActualStartupAt==b.ActualStartupAt && a.TargetTaxiTime==b.TargetTaxiTime && a.ArrivalGate==b.ArrivalGate && a.DepartureGate==b.DepartureGate && a.EstimatedLandingAt==b.EstimatedLandingAt && a.ActualStartRequestAt==b.ActualStartRequestAt && a.SourceRowIndex==b.SourceRowIndex && a.ArrivalIcaoFlightNumber==b.ArrivalIcaoFlightNumber && a.DepartureIcaoFlightNumber==b.DepartureIcaoFlightNumber && a.EstimatedDepartureAtSource==b.EstimatedDepartureAtSource && a.EstimatedArrivalAt==b.EstimatedArrivalAt && a.TargetOffBlockCount==b.TargetOffBlockCount && a.TargetStartupApprovalCount==b.TargetStartupApprovalCount;
}
