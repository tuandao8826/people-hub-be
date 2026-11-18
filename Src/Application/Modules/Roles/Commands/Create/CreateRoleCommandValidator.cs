using FluentValidation;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(role => role.Code).NotEmpty().MaximumLength(255);
        RuleFor(role => role.Name).NotEmpty().MaximumLength(255);
        RuleFor(role => role.Description).MaximumLength(255);
        RuleFor(role => role.PermissionCodes).NotEmpty();
    }
}
