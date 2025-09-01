using Application.Common.Extensions;
using Core.Bases;
using Core.Bases.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Modules.Roles.Entities;

public class Role : BaseEntity, ICode
{
	public string? Code { get; set; } = default!;

	public string Name { get; set; } = default!;

	public string? Description { get; set; }

	public ICollection<RolePermission>? RolePermissions { get; set; }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.UseDefaultTableNaming().HasBaseEntity();

		builder.Property(x => x.Code).IsRequired();

		builder.HasCitextUnique(x => x.Code);
	}
}
