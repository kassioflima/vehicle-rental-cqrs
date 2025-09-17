using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vehicle_rental.domain.Domain.deliveryPersons.entity;

namespace vehicle_rental.data.postgresql.context.configurations;

public class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
{
    public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Cnpj).IsUnique();
        builder.HasIndex(e => e.LicenseNumber).IsUnique();

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.Cnpj)
            .HasMaxLength(18)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(e => e.BirthDate)
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

        // Configure relationship with Rentals
        builder.HasMany(e => e.Rentals)
            .WithOne(r => r.DeliveryPerson)
            .HasForeignKey(r => r.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
