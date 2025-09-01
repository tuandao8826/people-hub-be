using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace Application.Common.Extensions;

public static class FluentValidationExtension
{
    private static readonly string[] sourceArray = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilder<T, TProperty> builder, string errorMessage)
    {
        throw new BadRequestException(errorMessage);
    }

    public static IRuleBuilderOptions<T, IFormFile?> MustBeImage<T>(this IRuleBuilder<T, IFormFile?> builder, string[]? sources = null)
        => builder.Must(file => file is null || (sources ?? sourceArray).Contains(Path.GetExtension(file.FileName).ToLower()));

    public static IRuleBuilderOptions<T, IEnumerable<TProperty>?> NotDuplicate<T, TProperty>(this IRuleBuilder<T, IEnumerable<TProperty>?> builder)
        => builder.Must(collection =>
        {
            return collection is null || collection.Distinct().Count() == collection.Count();
        });
}
