using Application.Common.Definitions.AclPermissions;
using Application.Common.Extensions;
using AutoMapper;
using Core.Bases.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Modules.Roles.Entities;

public class Permission(string code, string name) : ICode
{
	public string? Code { get; set; } = code;

	public string Name { get; set; } = name!;

	public string? Description { get; set; }

	[Column(TypeName = "jsonb")]
	public List<AclUserType> AllowedUserTypes { get; set; } = default!;

	public ICollection<RolePermission>? RolePermissions { get; set; }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure(EntityTypeBuilder<Permission> builder)
	{
		builder.UseDefaultTableNaming();

		builder.HasKey(x => x.Code);

		builder.HasCitextUnique(x => x.Code);
	}
}

public class PermissionMapping : Profile
{
    public PermissionMapping()
    {
		CreateMap<AclPermission, Permission>();
    }
}