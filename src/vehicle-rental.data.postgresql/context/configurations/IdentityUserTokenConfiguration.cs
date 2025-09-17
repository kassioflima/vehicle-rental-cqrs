using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("AspNetUserTokens");

        // Configure columns
        builder.Property(ut => ut.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid");

        builder.Property(ut => ut.LoginProvider)
            .HasColumnName("LoginProvider")
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(ut => ut.Name)
            .HasColumnName("Name")
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(ut => ut.Value)
            .HasColumnName("Value")
            .HasColumnType("text");

        // Primary key
        builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
    }
}
