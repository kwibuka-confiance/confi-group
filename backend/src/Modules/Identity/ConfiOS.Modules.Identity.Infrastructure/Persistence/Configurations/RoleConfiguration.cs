using ConfiOS.Modules.Identity.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Role"/> and its granted permissions.</summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("roles");
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name).HasMaxLength(100).IsRequired();

        builder.HasIndex(role => new { role.TenantId, role.Name }).IsUnique();

        builder.OwnsMany(role => role.Permissions, permission =>
        {
            permission.ToTable("role_permissions");
            permission.WithOwner().HasForeignKey(item => item.RoleId);
            permission.HasKey(item => new { item.RoleId, item.Permission });
            permission.Property(item => item.Permission).HasMaxLength(150);
        });

    }
}
