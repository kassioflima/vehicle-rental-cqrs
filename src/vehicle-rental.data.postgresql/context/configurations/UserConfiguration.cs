using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vehicle_rental.domain.Domain.auth.entity;

namespace vehicle_rental.data.postgresql.context.configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers");

        // Configure string columns to use varchar instead of nvarchar
        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid");

        builder.Property(u => u.UserName)
            .HasColumnName("UserName")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.NormalizedUserName)
            .HasColumnName("NormalizedUserName")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.NormalizedEmail)
            .HasColumnName("NormalizedEmail")
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.EmailConfirmed)
            .HasColumnName("EmailConfirmed")
            .HasColumnType("boolean");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("text");

        builder.Property(u => u.SecurityStamp)
            .HasColumnName("SecurityStamp")
            .HasColumnType("text");

        builder.Property(u => u.ConcurrencyStamp)
            .HasColumnName("ConcurrencyStamp")
            .HasColumnType("text");

        builder.Property(u => u.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("text");

        builder.Property(u => u.PhoneNumberConfirmed)
            .HasColumnName("PhoneNumberConfirmed")
            .HasColumnType("boolean");

        builder.Property(u => u.TwoFactorEnabled)
            .HasColumnName("TwoFactorEnabled")
            .HasColumnType("boolean");

        builder.Property(u => u.LockoutEnd)
            .HasColumnName("LockoutEnd")
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.LockoutEnabled)
            .HasColumnName("LockoutEnabled")
            .HasColumnType("boolean");

        builder.Property(u => u.AccessFailedCount)
            .HasColumnName("AccessFailedCount")
            .HasColumnType("integer");

        // Custom properties
        builder.Property(u => u.Role)
            .HasColumnName("Role")
            .HasColumnType("integer");

        builder.Property(u => u.DeliveryPersonId)
            .HasColumnName("DeliveryPersonId")
            .HasColumnType("uuid");

        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("boolean")
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("UserNameIndex")
            .IsUnique();

        builder.HasIndex(u => u.NormalizedEmail)
            .HasDatabaseName("EmailIndex");

        builder.HasIndex(u => u.DeliveryPersonId)
            .HasDatabaseName("IX_AspNetUsers_DeliveryPersonId");
    }
}
