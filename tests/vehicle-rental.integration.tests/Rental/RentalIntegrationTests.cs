using Microsoft.EntityFrameworkCore;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.integration.tests.Infrastructure;

namespace vehicle_rental.integration.tests.Rental;

public class RentalIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateRental_ValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase1");

        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "João Silva",
            "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            ELicenseType.A
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var rental = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7),
            50.00m
        );

        // Act
        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        // Assert
        var savedRental = await context.Rentals
            .FirstOrDefaultAsync(r => r.DeliveryPersonId == deliveryPerson.Id && r.MotorcycleId == motorcycle.Id);

        savedRental.Should().NotBeNull();
        savedRental!.DailyRate.Should().Be(50.00m);
        savedRental.Plan.Should().Be(ERentalPlan.SevenDays);
    }

    [Fact]
    public async Task GetRentalById_ExistingId_ShouldReturnRental()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase2");

        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Maria Santos",
            "98.765.432/0001-10",
            new DateTime(1985, 8, 20),
            "987654321",
            ELicenseType.AB
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2022",
            "Yamaha MT-07",
            "XYZ-9876"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var rental = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle.Id,
            ERentalPlan.FifteenDays,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(15),
            DateTime.UtcNow.AddDays(15),
            45.00m
        );

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        // Act
        var foundRental = await context.Rentals
            .FirstOrDefaultAsync(r => r.Id == rental.Id);

        // Assert
        foundRental.Should().NotBeNull();
        foundRental!.Plan.Should().Be(ERentalPlan.FifteenDays);
        foundRental.DailyRate.Should().Be(45.00m);
    }

    [Fact]
    public async Task GetAllRentals_ShouldReturnAllRentals()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase3");

        var deliveryPerson1 = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Pedro Costa",
            "11.222.333/0001-44",
            new DateTime(1992, 3, 10),
            "111222333",
            ELicenseType.A
        );

        var deliveryPerson2 = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Ana Lima",
            "55.666.777/0001-88",
            new DateTime(1988, 12, 5),
            "555666777",
            ELicenseType.AB
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Kawasaki Ninja",
            "DEF-5678"
        );

        context.DeliveryPersons.AddRange(deliveryPerson1, deliveryPerson2);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var rental1 = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson1.Id,
            motorcycle.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7),
            50.00m
        );

        var rental2 = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson2.Id,
            motorcycle.Id,
            ERentalPlan.FifteenDays,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(16),
            DateTime.UtcNow.AddDays(16),
            45.00m
        );

        context.Rentals.AddRange(rental1, rental2);
        await context.SaveChangesAsync();

        // Act
        var allRentals = await context.Rentals.ToListAsync();

        // Assert
        allRentals.Should().HaveCount(2);
        allRentals.Should().Contain(r => r.DeliveryPersonId == deliveryPerson1.Id);
        allRentals.Should().Contain(r => r.DeliveryPersonId == deliveryPerson2.Id);
    }

    [Fact]
    public async Task CalculateRentalReturn_EarlyReturn_ShouldCalculateFine()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase4");

        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Carlos Oliveira",
            "99.888.777/0001-66",
            new DateTime(1995, 7, 22),
            "999888777",
            ELicenseType.A
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var rental = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow.AddDays(-7), // Started 7 days ago
            DateTime.UtcNow.AddDays(-2), // Ended 2 days ago (early return)
            DateTime.UtcNow, // Expected to end today
            50.00m
        );

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        // Act
        var calculation = rental.CalculateReturnAmount(DateTime.UtcNow.AddDays(-2));

        // Assert
        calculation.DaysUsed.Should().Be(5); // 7 - 2 = 5 days used
        calculation.DaysRemaining.Should().Be(2); // 7 - 5 = 2 days remaining
        calculation.FineAmount.Should().BeGreaterThan(0); // Should have fine for early return
        calculation.TotalAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CompleteRental_ValidEndDate_ShouldCompleteSuccessfully()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase5");

        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Roberto Silva",
            "44.333.222/0001-11",
            new DateTime(1990, 1, 1),
            "444333222",
            ELicenseType.A
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Yamaha MT-07",
            "XYZ-9876"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var rental = new vehicle_rental.domain.Domain.rentals.entity.Rental(
            deliveryPerson.Id,
            motorcycle.Id,
            ERentalPlan.SevenDays,
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow,
            50.00m
        );

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        // Act
        rental.CompleteRental(DateTime.UtcNow.AddDays(-2));
        await context.SaveChangesAsync();

        // Assert
        var completedRental = await context.Rentals
            .FirstOrDefaultAsync(r => r.Id == rental.Id);

        completedRental.Should().NotBeNull();
        completedRental!.Status.Should().Be(RentalStatus.Completed);
        completedRental.EndDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-2), TimeSpan.FromSeconds(1));
    }
}