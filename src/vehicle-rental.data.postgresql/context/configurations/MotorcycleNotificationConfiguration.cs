using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vehicle_rental.domain.Domain.motorcycleNotifications.entity;

namespace vehicle_rental.data.postgresql.context.configurations;

public class MotorcycleNotificationConfiguration : IEntityTypeConfiguration<MotorcycleNotification>
{
    public void Configure(EntityTypeBuilder<MotorcycleNotification> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.MotorcycleId)
            .IsRequired();

        builder.Property(e => e.Year)
            .HasMaxLength(4)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.Model)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.LicensePlate)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.NotificationDate)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false)
            .HasConversion(
                v => v.HasValue && v.Value.Kind == DateTimeKind.Utc ? v : v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);
    }
}
