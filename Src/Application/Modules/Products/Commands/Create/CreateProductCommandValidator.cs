using Application.Common.Interfaces.Persistence;
using Application.Modules.Products.Bases.Requests.Products;
using FluentValidation;

namespace Application.Modules.Products.Commands.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IUnitOfWork unitOfWork)
    {
        Include(new ProductValidator(unitOfWork));
    }
}
