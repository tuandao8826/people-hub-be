using FluentValidation;
using System.ComponentModel.DataAnnotations;

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
        RuleFor(request => request.Phone).MaximumLength(30);
        RuleFor(request => request.Department).MaximumLength(100);
        RuleFor(request => request.JobTitle).MaximumLength(100);
        RuleFor(request => request.Status).IsInEnum();
    }
}
