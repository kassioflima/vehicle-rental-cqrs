using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("AspNetUserClaims");

        // Configure columns
        builder.Property(uc => uc.Id)
            .HasColumnName("Id")
            .HasColumnType("integer");

        builder.Property(uc => uc.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid");

        builder.Property(uc => uc.ClaimType)
            .HasColumnName("ClaimType")
            .HasColumnType("text");

        builder.Property(uc => uc.ClaimValue)
            .HasColumnName("ClaimValue")
            .HasColumnType("text");

        // Primary key
        builder.HasKey(uc => uc.Id);

        // Indexes
        builder.HasIndex(uc => uc.UserId)
            .HasDatabaseName("IX_AspNetUserClaims_UserId");
    }
}
