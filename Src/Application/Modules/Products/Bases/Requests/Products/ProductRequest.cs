using Application.Common.Attributes;
using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Categories.Entities;
using Application.Modules.Products.Entities;
using AutoMapper;
using Core.Common.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Modules.Products.Bases.Requests.Products;

[DisplayMessage(nameof(Product))]
public abstract class ProductRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public decimal? Price { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// Ex: Kg, Cái,...
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Ảnh đại diện
    /// </summary>
    public IFormFile? ThumbnailFile { get; set; }

    /// <summary>
    /// Loại sản phẩm
    /// </summary>
    public Guid? CategoryId { get; set; }

    public OperationStatus OperationStatus { get; set; } = OperationStatus.Active;
}

public class ProductValidator : AbstractValidator<ProductRequest>
{
    public ProductValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(MessageErrorType.Required)
            .MaximumLength(255)
            .WithMessage(MessageErrorType.TooLong);

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage(MessageErrorType.TooLong)
            .When(x => string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(MessageErrorType.TooShort)
            .When(x => x.Price is not null);

        RuleFor(x => x.Unit)
            .MaximumLength(255)
            .WithMessage(MessageErrorType.TooLong)
            .When(x => string.IsNullOrEmpty(x.Unit));

        RuleFor(x => x.CategoryId)
            .Must(id => unitOfWork.HasId<Category>(id!.Value))
            .WithMessage(MessageErrorType.NotFound)
            .When(x => x.CategoryId is not null);

        RuleFor(x => x.ThumbnailFile)
            .MustBeImage()
            .WithMessage(MessageErrorType.Invalid)
            .When(x => x.ThumbnailFile is not null);

        RuleFor(x => x.OperationStatus)
            .NotEmpty()
            .WithMessage(MessageErrorType.Required)
            .IsInEnum()
            .WithMessage(MessageErrorType.Invalid);
    }
}

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<ProductRequest, Product>();
    }
}