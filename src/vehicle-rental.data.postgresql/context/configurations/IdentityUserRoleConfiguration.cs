using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("AspNetUserRoles");

        // Configure columns
        builder.Property(ur => ur.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid");

        builder.Property(ur => ur.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("uuid");

        // Primary key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Indexes
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_AspNetUserRoles_RoleId");
    }
}
