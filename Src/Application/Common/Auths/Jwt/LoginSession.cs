using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Common.Auths.Jwt;

public sealed class LoginSession
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
}

internal sealed class LoginSessionConfiguration : IEntityTypeConfiguration<LoginSession>
{
    public void Configure(EntityTypeBuilder<LoginSession> builder)
    {
        builder.ToTable("login_session");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        builder.HasIndex(session => session.ExpiresAt);
    }
}
