using Application.Common.Extensions;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Products.Entities;

public class ProductImage : BaseEntity
{
    public string Path { get; set; } = default!;

    public Guid ProductId { get; set; }

    public Product? Product { get; set; }

    public int Order { get; set; } = 1;
}

public class ProductImageConfigurations : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder
            .HasBaseEntity()
            .UseDefaultTableNaming();

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}