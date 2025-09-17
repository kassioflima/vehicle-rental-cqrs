using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("AspNetRoleClaims");

        // Configure columns
        builder.Property(rc => rc.Id)
            .HasColumnName("Id")
            .HasColumnType("integer");

        builder.Property(rc => rc.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("uuid");

        builder.Property(rc => rc.ClaimType)
            .HasColumnName("ClaimType")
            .HasColumnType("text");

        builder.Property(rc => rc.ClaimValue)
            .HasColumnName("ClaimValue")
            .HasColumnType("text");

        // Primary key
        builder.HasKey(rc => rc.Id);

        // Indexes
        builder.HasIndex(rc => rc.RoleId)
            .HasDatabaseName("IX_AspNetRoleClaims_RoleId");
    }
}
