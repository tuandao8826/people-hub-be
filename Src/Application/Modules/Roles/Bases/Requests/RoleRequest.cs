using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Roles.Entities;
using Application.Modules.Roles.Validations;
using AutoMapper;
using FluentValidation;

namespace Application.Modules.Roles.Bases.Requests;

public class RoleRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public ICollection<string>? PermissionCodes { get; set; }
}

public class RoleValidator : AbstractValidator<RoleRequest>
{
    public RoleValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Message<Role>.Required(x => x.Name))
            .MaximumLength(255)
            .WithMessage(Message<Role>.TooLong(x => x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage(Message<Role>.TooLong(x => x.Description));

        RuleFor(x => x.PermissionCodes)
            .NotDuplicate()
            .WithMessage(Message<Role>.Duplicate(x => x.RolePermissions))
            .Must(codes => unitOfWork.HasPermissionCodes(codes!))
            .WithMessage(Message<Permission>.NotFound())
            .When(x => x.PermissionCodes != null && x.PermissionCodes.Count > 0);
    }
}

public partial class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<RoleRequest, Role>();
    }
}
