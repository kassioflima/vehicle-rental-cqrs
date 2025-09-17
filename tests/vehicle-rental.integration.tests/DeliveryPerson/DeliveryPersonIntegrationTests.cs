using Microsoft.EntityFrameworkCore;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.integration.tests.Infrastructure;

namespace vehicle_rental.integration.tests.DeliveryPerson;

public class DeliveryPersonIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateDeliveryPerson_ValidData_ShouldCreateSuccessfully()
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

        // Act
        context.DeliveryPersons.Add(deliveryPerson);
        await context.SaveChangesAsync();

        // Assert
        var savedDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Cnpj == "12.345.678/0001-90");

        savedDeliveryPerson.Should().NotBeNull();
        savedDeliveryPerson!.Name.Should().Be("João Silva");
        savedDeliveryPerson.LicenseType.Should().Be(ELicenseType.A);
    }

    [Fact]
    public async Task GetDeliveryPersonById_ExistingId_ShouldReturnDeliveryPerson()
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

        context.DeliveryPersons.Add(deliveryPerson);
        await context.SaveChangesAsync();

        // Act
        var foundDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Id == deliveryPerson.Id);

        // Assert
        foundDeliveryPerson.Should().NotBeNull();
        foundDeliveryPerson!.Name.Should().Be("Maria Santos");
        foundDeliveryPerson.Cnpj.Should().Be("98.765.432/0001-10");
    }

    [Fact]
    public async Task GetAllDeliveryPersons_ShouldReturnAllDeliveryPersons()
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

        context.DeliveryPersons.AddRange(deliveryPerson1, deliveryPerson2);
        await context.SaveChangesAsync();

        // Act
        var allDeliveryPersons = await context.DeliveryPersons.ToListAsync();

        // Assert
        allDeliveryPersons.Should().HaveCount(2);
        allDeliveryPersons.Should().Contain(dp => dp.Name == "Pedro Costa");
        allDeliveryPersons.Should().Contain(dp => dp.Name == "Ana Lima");
    }

    [Fact]
    public async Task UpdateLicenseImage_ValidImage_ShouldUpdateSuccessfully()
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

        context.DeliveryPersons.Add(deliveryPerson);
        await context.SaveChangesAsync();

        // Act
        deliveryPerson.UpdateLicenseImage("https://example.com/license.jpg");
        await context.SaveChangesAsync();

        // Assert
        var updatedDeliveryPerson = await context.DeliveryPersons
            .FirstOrDefaultAsync(dp => dp.Id == deliveryPerson.Id);

        updatedDeliveryPerson.Should().NotBeNull();
        updatedDeliveryPerson!.LicenseImageUrl.Should().Be("https://example.com/license.jpg");
    }

    [Fact]
    public async Task CanRentMotorcycle_ValidLicense_ShouldReturnTrue()
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

        context.DeliveryPersons.Add(deliveryPerson);
        await context.SaveChangesAsync();

        // Act
        var canRent = deliveryPerson.CanRentMotorcycle();

        // Assert
        canRent.Should().BeTrue();
    }
}