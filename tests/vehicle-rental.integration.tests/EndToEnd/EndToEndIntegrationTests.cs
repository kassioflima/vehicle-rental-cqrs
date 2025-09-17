using Microsoft.EntityFrameworkCore;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.integration.tests.Infrastructure;

namespace vehicle_rental.integration.tests.EndToEnd;

public class EndToEndIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CompleteRentalFlow_FromCreationToCompletion_ShouldWorkCorrectly()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase1");

        // Create delivery person
        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "João Silva",
            "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            ELicenseType.A
        );

        // Create motorcycle
        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act - Create rental
        var rental = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7),
            50.00m
        );

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        // Act - Complete rental
        rental.CompleteRental(DateTime.UtcNow.AddDays(5)); // Early return
        await context.SaveChangesAsync();

        // Assert
        var completedRental = await context.Rentals
            .FirstOrDefaultAsync(r => r.Id == rental.Id);

        completedRental.Should().NotBeNull();
        completedRental!.Status.Should().Be(RentalStatus.Completed);
        completedRental.EndDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(5), TimeSpan.FromSeconds(1));

        // Verify calculation
        var calculation = rental.CalculateReturnAmount(DateTime.UtcNow.AddDays(5));
        calculation.DaysUsed.Should().Be(5);
        calculation.DaysRemaining.Should().Be(2);
        calculation.FineAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MultipleRentalsFlow_ShouldHandleCorrectly()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase2");

        // Create delivery person
        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Maria Santos",
            "98.765.432/0001-10",
            new DateTime(1985, 8, 20),
            "987654321",
            ELicenseType.AB
        );

        // Create motorcycles
        var motorcycle1 = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        var motorcycle2 = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2022",
            "Yamaha MT-07",
            "XYZ-9876"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.AddRange(motorcycle1, motorcycle2);
        await context.SaveChangesAsync();

        // Act - Create multiple rentals
        var rental1 = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle1.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7),
            50.00m
        );

        var rental2 = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle2.Id,
            ERentalPlan.FifteenDays,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(16),
            DateTime.UtcNow.AddDays(16),
            45.00m
        );

        context.Rentals.AddRange(rental1, rental2);
        await context.SaveChangesAsync();

        // Assert
        var allRentals = await context.Rentals
            .Where(r => r.DeliveryPersonId == deliveryPerson.Id)
            .ToListAsync();

        allRentals.Should().HaveCount(2);
        allRentals.Should().Contain(r => r.MotorcycleId == motorcycle1.Id);
        allRentals.Should().Contain(r => r.MotorcycleId == motorcycle2.Id);
        allRentals.Should().AllSatisfy(r => r.Status.Should().Be(RentalStatus.Active));
    }

    [Fact]
    public async Task BusinessLogicValidation_ShouldWorkCorrectly()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase3");

        // Create delivery person
        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Pedro Costa",
            "11.222.333/0001-44",
            new DateTime(1992, 3, 10),
            "111222333",
            ELicenseType.A
        );

        // Create motorcycle
        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Kawasaki Ninja",
            "DEF-5678"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act & Assert - Test business logic
        var canRent = deliveryPerson.CanRentMotorcycle();
        canRent.Should().BeTrue();

        // Test license image update
        deliveryPerson.UpdateLicenseImage("https://example.com/license.jpg");
        deliveryPerson.LicenseImageUrl.Should().Be("https://example.com/license.jpg");

        // Test license plate update
        motorcycle.UpdateLicensePlate("NEW-9999");
        motorcycle.LicensePlate.Should().Be("NEW-9999");

        await context.SaveChangesAsync();

        // Verify changes were persisted
        var updatedDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Id == deliveryPerson.Id);
        var updatedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == motorcycle.Id);

        updatedDeliveryPerson!.LicenseImageUrl.Should().Be("https://example.com/license.jpg");
        updatedMotorcycle!.LicensePlate.Should().Be("NEW-9999");
    }
}