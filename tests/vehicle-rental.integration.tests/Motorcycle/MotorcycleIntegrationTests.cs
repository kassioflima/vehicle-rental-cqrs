using Microsoft.EntityFrameworkCore;
using vehicle_rental.integration.tests.Infrastructure;

namespace vehicle_rental.integration.tests.Motorcycle;

public class MotorcycleIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateMotorcycle_ValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase1");

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Honda CB 600F",
            "ABC-1234"
        );

        // Act
        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Assert
        var savedMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.LicensePlate == "ABC-1234");

        savedMotorcycle.Should().NotBeNull();
        savedMotorcycle!.Year.Should().Be("2023");
        savedMotorcycle.Model.Should().Be("Honda CB 600F");
    }

    [Fact]
    public async Task GetMotorcycleById_ExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase2");

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2022",
            "Yamaha MT-07",
            "XYZ-9876"
        );

        context.Motorcycles.Add(motorcycle);
        await context.SaveChangesAsync();

        // Act
        var foundMotorcycle = await context.Motorcycles
            .FirstOrDefaultAsync(m => m.Id == motorcycle.Id);

        // Assert
        foundMotorcycle.Should().NotBeNull();
        foundMotorcycle!.Model.Should().Be("Yamaha MT-07");
        foundMotorcycle.LicensePlate.Should().Be("XYZ-9876");
    }

    [Fact]
    public async Task GetAllMotorcycles_ShouldReturnAllMotorcycles()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase_GetAllMotorcycles");

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

        context.Motorcycles.AddRange(motorcycle1, motorcycle2);
        await context.SaveChangesAsync();

        // Act
        var allMotorcycles = await context.Motorcycles.ToListAsync();

        // Assert
        allMotorcycles.Should().HaveCount(2);
        allMotorcycles.Should().Contain(m => m.Model == "Honda CB 600F");
        allMotorcycles.Should().Contain(m => m.Model == "Yamaha MT-07");
    }

    [Fact]
    public async Task UpdateLicensePlate_ValidPlate_ShouldUpdateSuccessfully()
    {
        // Arrange
        using var context = CreateDbContext("TestDatabase4");

        var motorcycle = new vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle(
            "2023",
            "Kawasaki Ninja",
            "DEF-5678"
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
}