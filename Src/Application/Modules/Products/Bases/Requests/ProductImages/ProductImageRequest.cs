using Application.Common.Attributes;
using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Modules.Products.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Modules.Products.Bases.Requests.ProductImages;

[DisplayMessage(nameof(ProductImage))]
public abstract class ProductImageRequest
{
    public IFormFile? File { get; set; }

    public int Order { get; set; } = 1;
}

public class ProductImageValidator : AbstractValidator<ProductImageRequest>
{
    public ProductImageValidator()
    {
        RuleFor(x => x.File)
            .NotEmpty()
            .WithMessage(MessageErrorType.Required)
            .MustBeImage()
            .WithMessage(MessageErrorType.Invalid);

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage(MessageErrorType.TooShort);
    }
}
public class ProductImageMapping : Profile
{
    public ProductImageMapping()
    {
        CreateMap<ProductImageRequest, ProductImage>();
    }
}
