using Application.Common.Interfaces.Persistence;
using Application.Common.Requests;
using Application.Modules.Products.Entities;
using FluentValidation;

namespace Application.Modules.Products.Commands.DeleteMany;

public class DeleteProductsCommandValidator : AbstractValidator<DeleteProductsCommand>
{
    public DeleteProductsCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new MultipleIdentiferValidator<Product, Guid>(unitOfWork));
    }
}
