using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Categories.Bases.Requests;
using Application.Modules.Categories.Entities;
using FluentValidation;

namespace Application.Modules.Categories.Commands.Create;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new CategoryRequestValidator());

        RuleFor(x => x.Name)
            .Must(name => !unitOfWork.Exists<Category>(x => x.Name.ToLower().Equals(name.ToLower())))
            .WithMessage(Message<Category>.AlreadyExists(x => x.Name));

        RuleFor(x => x.ParentId)
            .Must(id => unitOfWork.HasId<Category>(id!.Value))
            .WithMessage(Message<Category>.NotFound(x => x.ParentId))
            .When(x => x.ParentId.HasValue);
    }
}
