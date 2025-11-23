using Application.Common.Interfaces.Persistence;
using Application.Common.Requests;
using Application.Modules.Categories.Entities;
using FluentValidation;

namespace Application.Modules.Categories.Commands.DeleteMany;

public class DeleteCategoriesCommandValidator : AbstractValidator<DeleteCategoriesCommand>
{
    public DeleteCategoriesCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new MultipleIdentiferValidator<Category, Guid>(unitOfWork));
    }
}
