using Application.Common.Extensions;
using Application.Modules.Categories.Entities;
using Core.Bases;
using Core.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Products.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;

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
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Loại sản phẩm
    /// </summary>
    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }

    public OperationStatus OperationStatus { get; set; } = OperationStatus.Active;

    public ICollection<ProductImage>? Images { get; set; }
}

//public class ProductConfigurations : IEntityTypeConfiguration<Product>
//{
//    public void Configure(EntityTypeBuilder<Product> builder)
//    {
//        builder.HasBaseEntity().UseDefaultTableNaming();

//        builder
//            .HasOne(x => x.Category)
//            .WithMany(x => x.Products)
//            .HasForeignKey(x => x.CategoryId)
//            .OnDelete(DeleteBehavior.SetNull);
//    }
//}
