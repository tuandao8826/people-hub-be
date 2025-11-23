using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Modules.Categories.Entities;
using FluentValidation;

namespace Application.Modules.Categories.Bases.Requests;

public abstract class CategoryRequest
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Message<Category>.Required(x => x.Name))
            .MaximumLength(255)
            .WithMessage(Message<Category>.TooLong(x => x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage(Message<Category>.TooLong(x => x.Description))
            .When(x => string.IsNullOrEmpty(x.Description));
    }
}