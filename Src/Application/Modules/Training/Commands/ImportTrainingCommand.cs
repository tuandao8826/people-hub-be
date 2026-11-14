using System.ComponentModel.DataAnnotations;
using Application.Modules.Subjects.Entites;
using MediatR;

namespace Application.Modules.Training.Commands;

public record ImportTrainingCommand(
    [Required, MinLength(1)] IReadOnlyList<ImportTrainingEmployee> Employees,
    [Required, MinLength(1)] IReadOnlyList<ImportTrainingSubject> Subjects,
    [Required] IReadOnlyList<ImportTrainingEntry> Entries,
    bool PreviewOnly = false,
    ImportTrainingResult? ExpectedResult = null) : IRequest<ImportTrainingResult>;

public record ImportTrainingEmployee(
    [Required, StringLength(30)] string Code,
    [Required, StringLength(100)] string Name,
    bool UsesTemporaryCode = false);

public record ImportTrainingSubject(
    [Required, StringLength(50)] string Code,
    [Required, StringLength(255)] string Name,
    [Range(1, 9999)] int CycleValue,
    [EnumDataType(typeof(SubjectCycleUnit))] SubjectCycleUnit CycleUnit);

public record ImportTrainingEntry(
    [Required, StringLength(30)] string StaffCode,
    [Required, StringLength(50)] string SubjectCode,
    DateOnly TrainingDate);

public record ImportTrainingResult(
    int EmployeesCreated,
    int SubjectsCreated,
    int RecordsCreated,
    int RecordsUpdated,
    int RecordsUnchanged,
    int RecordsSkipped);
