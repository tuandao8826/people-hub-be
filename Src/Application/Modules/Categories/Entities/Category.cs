using Application.Common.Extensions;
using Application.Modules.Products.Entities;
using Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Categories.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    /// <summary>
    /// Danh mục cha
    /// </summary>
    public Guid? ParentId { get; set; }

    public Category? Parent { get; set; }

    /// <summary>
    /// Đường dẫn phân cấp
    /// Note: '{Parent_HierarchyPath}~{Current_HierarchyPath}' 
    /// </summary>
    public string HierarchyPath { get; set; } = default!;

    /// <summary>
    /// Độ sâu đường dẫn
    /// </summary>
    public int HierarchyDeep { get; set; } = 1;

    public ICollection<Category>? Children { get; set; }

    public ICollection<Product>? Products { get; set; }

    public Category SetHierarchy(string? parentPath = null)
    {
        if (string.IsNullOrEmpty(parentPath))
        {
            HierarchyPath = Id.ToString();
            HierarchyDeep = 1;
        }
        else
        {
            HierarchyPath = $"{parentPath}~{Id}";
            HierarchyDeep = parentPath.Split('~').Length + 1;
        }

        return this;
    }
}

//public class CategoryConfigurations : IEntityTypeConfiguration<Category>
//{
//    public void Configure(EntityTypeBuilder<Category> builder)
//    {
//        builder.HasBaseEntity().UseDefaultTableNaming();

//        builder
//            .HasOne(x => x.Parent)
//            .WithMany(x => x.Children)
//            .HasForeignKey(x => x.ParentId)
//            .OnDelete(DeleteBehavior.Cascade);
//    }
//}