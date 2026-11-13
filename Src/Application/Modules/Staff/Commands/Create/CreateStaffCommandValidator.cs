using Application.Modules.Staff.Bases.Requests;
using FluentValidation;

namespace Application.Modules.Staff.Commands.Create;

public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator(StaffRequestValidator staffRequestValidator)
    {
        Include(staffRequestValidator);
    }
}
