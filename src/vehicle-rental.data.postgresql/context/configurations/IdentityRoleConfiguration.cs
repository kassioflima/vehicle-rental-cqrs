using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vehicle_rental.data.postgresql.context.configurations;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("AspNetRoles");

        // Configure string columns to use varchar instead of nvarchar
        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid");

        builder.Property(r => r.Name)
            .HasColumnName("Name")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(r => r.NormalizedName)
            .HasColumnName("NormalizedName")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(r => r.ConcurrencyStamp)
            .HasColumnName("ConcurrencyStamp")
            .HasColumnType("text");

        // Indexes
        builder.HasIndex(r => r.NormalizedName)
            .HasDatabaseName("RoleNameIndex")
            .IsUnique();
    }
}
