using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Modules.Staff.Bases.Requests;

public class StaffRequestValidator : AbstractValidator<StaffRequest>
{
    public StaffRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;

        RuleFor(request => request.Code).NotEmpty().MaximumLength(30);
        RuleFor(request => request.Name).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Email)
            .MaximumLength(254)
            .Must(new EmailAddressAttribute().IsValid)
            .WithMessage("The Email field is not a valid e-mail address.");
        RuleFor(request => request.Phone)
            .MaximumLength(30)
            .Must(phone => string.IsNullOrWhiteSpace(phone) || IsValidPhone(phone))
            .WithMessage("Số điện thoại phải có từ 7 đến 15 chữ số và đúng định dạng.");
        RuleFor(request => request.Department).MaximumLength(100);
        RuleFor(request => request.JobTitle).MaximumLength(100);
        RuleFor(request => request.Status).IsInEnum();
    }

    private static bool IsValidPhone(string phone)
    {
        var value = phone.Trim();
        return Regex.IsMatch(value, @"^\+?[0-9\s().-]+$") &&
               value.Count(char.IsDigit) is >= 7 and <= 15;
    }
}
