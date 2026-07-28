using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="User"/> and its role and branch assignments.</summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName).HasMaxLength(200).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(user => user.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(user => user.PreferredLanguage).HasMaxLength(10);

        // Stored as a converted single column rather than an owned type, so the composite
        // uniqueness index below can span TenantId and the email together.
        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired()
            .HasConversion(email => email.Value, value => EmailAddress.Create(value));

        builder.Property(user => user.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone!.Value,
                value => PhoneNumber.Create(value));

        // Unique per tenant, not globally: the same person may hold an account at two
        // businesses, and those accounts are independent.
        builder.HasIndex(user => new { user.TenantId, user.Email }).IsUnique();

        builder.OwnsMany(user => user.Roles, role =>
        {
            role.ToTable("user_roles");
            role.WithOwner().HasForeignKey(assignment => assignment.UserId);
            role.HasKey(assignment => new { assignment.UserId, assignment.RoleId });
        });

        builder.OwnsMany(user => user.Branches, branch =>
        {
            branch.ToTable("user_branches");
            branch.WithOwner().HasForeignKey(assignment => assignment.UserId);
            branch.HasKey(assignment => new { assignment.UserId, assignment.BranchId });
        });
    }
}
