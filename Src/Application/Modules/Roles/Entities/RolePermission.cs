using Application.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Roles.Entities;

public class RolePermission
{
	public RolePermission() { }

    public RolePermission(Guid roleId, string permissionCode)
    {
        RoleId = roleId;
		PermissionCode = permissionCode;
    }

    public Guid RoleId { get; set; }

	public Role? Role { get; set; }

	public string PermissionCode { get; set; } = default!;

	public Permission? Permission { get; set; }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
	public void Configure(EntityTypeBuilder<RolePermission> builder)
	{
		builder.UseDefaultTableNaming();

		builder.HasKey(x => new { x.RoleId, x.PermissionCode });

		builder
			.HasOne(x => x.Role)
			.WithMany(x => x.RolePermissions)
			.HasForeignKey(x => x.RoleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.Permission)
			.WithMany(x => x.RolePermissions)
			.OnDelete(DeleteBehavior.Cascade)
			.HasForeignKey(x => x.PermissionCode);
	}
}
