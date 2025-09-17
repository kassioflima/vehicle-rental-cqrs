using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("AspNetUserLogins");

        // Configure columns
        builder.Property(ul => ul.LoginProvider)
            .HasColumnName("LoginProvider")
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(ul => ul.ProviderKey)
            .HasColumnName("ProviderKey")
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(ul => ul.ProviderDisplayName)
            .HasColumnName("ProviderDisplayName")
            .HasColumnType("text");

        builder.Property(ul => ul.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid");

        // Primary key
        builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

        // Indexes
        builder.HasIndex(ul => ul.UserId)
            .HasDatabaseName("IX_AspNetUserLogins_UserId");
    }
}
