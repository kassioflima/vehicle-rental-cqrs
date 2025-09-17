using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vehicle_rental.data.postgresql.context;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.integration.tests;

public class BasicIntegrationTests
{
    [Fact]
    public async Task DatabaseIntegration_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase");
        });

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

        // Act & Assert - Test DeliveryPerson
        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "João Silva",
            "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            ELicenseType.A
        );

        context.DeliveryPersons.Add(deliveryPerson);
        await context.SaveChangesAsync();

        var savedDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Cnpj == "12.345.678/0001-90");

        savedDeliveryPerson.Should().NotBeNull();
        savedDeliveryPerson!.Name.Should().Be("João Silva");
        savedDeliveryPerson.LicenseType.Should().Be(ELicenseType.A);

        // Act & Assert - Test Motorcycle
        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        var savedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.LicensePlate == "ABC-1234");

        savedMotorcycle.Should().NotBeNull();
        savedMotorcycle!.Year.Should().Be("2023");
        savedMotorcycle.Model.Should().Be("Honda CB 600F");

        // Act & Assert - Test Rental
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

        var savedRental = await context.Rentals
            .FirstOrDefaultAsync(r => r.DeliveryPersonId == deliveryPerson.Id && r.MotorcycleId == motorcycle.Id);

        savedRental.Should().NotBeNull();
        savedRental!.DailyRate.Should().Be(50.00m);
        savedRental.Plan.Should().Be(ERentalPlan.SevenDays);
        savedRental.Status.Should().Be(RentalStatus.Active);
    }

    [Fact]
    public async Task RentalCalculation_ShouldWorkCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase2");
        });

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

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
    public async Task BusinessLogic_ShouldWorkCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase3");
        });

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();

        var deliveryPerson = new vehicle_rental.domain.Domain.deliveryPersons.entity.DeliveryPerson(
            "Pedro Costa",
            "11.222.333/0001-44",
            new DateTime(1992, 3, 10),
            "111222333",
            ELicenseType.A
        );

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Kawasaki Ninja",
            "DEF-5678"
        );

        context.DeliveryPersons.Add(deliveryPerson);
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act & Assert - Test CanRentMotorcycle
        var canRent = deliveryPerson.CanRentMotorcycle();
        canRent.Should().BeTrue();

        // Act & Assert - Test UpdateLicenseImage
        deliveryPerson.UpdateLicenseImage("https://example.com/license.jpg");
        deliveryPerson.LicenseImageUrl.Should().Be("https://example.com/license.jpg");

        // Act & Assert - Test UpdateLicensePlate
        motorcycle.UpdateLicensePlate("NEW-9999");
        motorcycle.LicensePlate.Should().Be("NEW-9999");

        await context.SaveChangesAsync();

        // Verify changes were saved
        var updatedDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Id == deliveryPerson.Id);
        var updatedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == motorcycle.Id);

        updatedDeliveryPerson!.LicenseImageUrl.Should().Be("https://example.com/license.jpg");
        updatedMotorcycle!.LicensePlate.Should().Be("NEW-9999");
    }
}