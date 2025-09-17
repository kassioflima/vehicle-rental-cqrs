using Microsoft.EntityFrameworkCore;
using vehicle_rental.integration.tests.Infrastructure;

namespace vehicle_rental.integration.tests.Messaging;

public class RabbitMQIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task MotorcycleEntity_ShouldHaveRequiredProperties()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase1");

        // Act
        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Assert
        motorcycle.Id.Should().NotBeEmpty();
        motorcycle.Year.Should().Be("2023");
        motorcycle.Model.Should().Be("Honda CB 600F");
        motorcycle.LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task MotorcycleUpdateLicensePlate_ShouldWorkCorrectly()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase2");

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act
        motorcycle.UpdateLicensePlate("NEW-9999");
        await context.SaveChangesAsync();

        // Assert
        var updatedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == motorcycle.Id);

        updatedMotorcycle.Should().NotBeNull();
        updatedMotorcycle!.LicensePlate.Should().Be("NEW-9999");
    }

    [Fact]
    public async Task MotorcycleRentals_ShouldBeManageable()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase3");

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Yamaha MT-07",
            "XYZ-9876"
        );

        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act & Assert - Check rentals collection
        motorcycle.Rentals.Should().NotBeNull();
        motorcycle.Rentals.Should().BeEmpty();

        // Verify motorcycle properties
        motorcycle.Year.Should().Be("2023");
        motorcycle.Model.Should().Be("Yamaha MT-07");
        motorcycle.LicensePlate.Should().Be("XYZ-9876");

        // Test license plate update
        motorcycle.UpdateLicensePlate("UPDATED-123");
        motorcycle.LicensePlate.Should().Be("UPDATED-123");

        await context.SaveChangesAsync();

        // Verify changes were persisted
        var updatedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == motorcycle.Id);

        updatedMotorcycle.Should().NotBeNull();
        updatedMotorcycle!.LicensePlate.Should().Be("UPDATED-123");
    }
}