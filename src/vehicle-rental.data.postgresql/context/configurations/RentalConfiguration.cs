using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vehicle_rental.domain.Domain.rentals.entity;

namespace vehicle_rental.data.postgresql.context.configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DeliveryPersonId)
            .IsRequired();

        builder.Property(e => e.MotorcycleId)
            .IsRequired();

        builder.Property(e => e.Plan)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(e => e.EndDate)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(e => e.ExpectedEndDate)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(e => e.DailyRate)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(e => e.TotalAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(e => e.FineAmount)
            .HasPrecision(10, 2)
            .IsRequired(false);

        builder.Property(e => e.AdditionalDaysAmount)
            .HasPrecision(10, 2)
            .IsRequired(false);

        builder.Property(e => e.Status)
            .IsRequired();

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

        // Configure relationship with DeliveryPerson
        builder.HasOne(e => e.DeliveryPerson)
            .WithMany(d => d.Rentals)
            .HasForeignKey(e => e.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with Motorcycle
        builder.HasOne(e => e.Motorcycle)
            .WithMany(m => m.Rentals)
            .HasForeignKey(e => e.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
