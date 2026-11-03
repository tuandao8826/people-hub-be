using Application.Common.Interfaces.Persistence;
using Application.Modules.Products.Bases.Requests.Products;
using FluentValidation;

namespace Application.Modules.Products.Commands.Update;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator(IUnitOfWork unitOfWork)
    {
        Include(new ProductValidator(unitOfWork));
    }
}