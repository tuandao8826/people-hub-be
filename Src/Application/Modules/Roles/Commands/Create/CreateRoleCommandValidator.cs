using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Bases.Requests;
using Application.Modules.Roles.Entities;
using FluentValidation;

namespace Application.Modules.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(Message<Role>.Required(x => x.Code))
            .MaximumLength(255)
            .WithMessage(Message<Role>.TooLong(x => x.Code))
            .Must(code => !unitOfWork.HasCode<Role>(code))
            .WithMessage(Message<Role>.AlreadyExists(x => x.Code));

        Include(new RoleValidator(unitOfWork));
    }
}
