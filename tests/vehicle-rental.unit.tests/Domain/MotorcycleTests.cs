using vehicle_rental.domain.Domain.motorcycles.entity;

namespace vehicle_rental.unit.tests.Domain;

public class MotorcycleTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateMotorcycleWithCorrectProperties()
    {
        // Arrange
        var year = "2024";
        var model = "Honda CB 600F";
        var licensePlate = "ABC-1234";

        // Act
        var motorcycle = new Motorcycle(year, model, licensePlate);

        // Assert
        motorcycle.Year.Should().Be(year);
        motorcycle.Model.Should().Be(model);
        motorcycle.LicensePlate.Should().Be(licensePlate);
        motorcycle.Id.Should().NotBe(Guid.Empty);
        motorcycle.CreatedAt.Should().NotBe(default(DateTime)); // Should have a valid timestamp
        motorcycle.Rentals.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void UpdateLicensePlate_ValidLicensePlate_ShouldUpdateLicensePlateAndTimestamp()
    {
        // Arrange
        var motorcycle = CreateTestMotorcycle();
        var newLicensePlate = "XYZ-5678";
        var originalUpdatedAt = motorcycle.UpdatedAt;

        // Act
        motorcycle.UpdateLicensePlate(newLicensePlate);

        // Assert
        motorcycle.LicensePlate.Should().Be(newLicensePlate);
        motorcycle.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    [Fact]
    public void UpdateLicensePlate_SameLicensePlate_ShouldStillUpdateTimestamp()
    {
        // Arrange
        var motorcycle = CreateTestMotorcycle();
        var originalLicensePlate = motorcycle.LicensePlate;
        var originalUpdatedAt = motorcycle.UpdatedAt;

        // Act
        motorcycle.UpdateLicensePlate(originalLicensePlate!);

        // Assert
        motorcycle.LicensePlate.Should().Be(originalLicensePlate);
        motorcycle.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    [Fact]
    public void UpdateLicensePlate_NullLicensePlate_ShouldUpdateToNull()
    {
        // Arrange
        var motorcycle = CreateTestMotorcycle();

        // Act
        motorcycle.UpdateLicensePlate(null!);

        // Assert
        motorcycle.LicensePlate.Should().BeNull();
    }

    [Fact]
    public void UpdateLicensePlate_EmptyLicensePlate_ShouldUpdateToEmpty()
    {
        // Arrange
        var motorcycle = CreateTestMotorcycle();

        // Act
        motorcycle.UpdateLicensePlate(string.Empty);

        // Assert
        motorcycle.LicensePlate.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("2020", "Honda CB 600F", "ABC-1234")]
    [InlineData("2024", "Yamaha FZ6", "XYZ-5678")]
    [InlineData("2019", "Kawasaki Ninja 650", "DEF-9012")]
    public void Constructor_DifferentParameters_ShouldCreateMotorcycleWithCorrectValues(string year, string model, string licensePlate)
    {
        // Act
        var motorcycle = new Motorcycle(year, model, licensePlate);

        // Assert
        motorcycle.Year.Should().Be(year);
        motorcycle.Model.Should().Be(model);
        motorcycle.LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public void Constructor_WithSpecialCharactersInModel_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var modelWithSpecialChars = "Honda CB 600F Hornet & CBR";
        var motorcycle = new Motorcycle("2024", modelWithSpecialChars, "ABC-1234");

        // Assert
        motorcycle.Model.Should().Be(modelWithSpecialChars);
    }

    [Fact]
    public void Constructor_WithFormattedLicensePlate_ShouldPreserveLicensePlateFormat()
    {
        // Arrange
        var formattedLicensePlate = "ABC-1234";
        var motorcycle = new Motorcycle("2024", "Honda CB 600F", formattedLicensePlate);

        // Assert
        motorcycle.LicensePlate.Should().Be(formattedLicensePlate);
    }

    [Fact]
    public void Constructor_WithLongModelName_ShouldPreserveLongModelName()
    {
        // Arrange
        var longModelName = "Honda CB 600F Hornet ABS Adventure Touring Edition";
        var motorcycle = new Motorcycle("2024", longModelName, "ABC-1234");

        // Assert
        motorcycle.Model.Should().Be(longModelName);
    }

    private static Motorcycle CreateTestMotorcycle()
    {
        return new Motorcycle("2024", "Honda CB 600F", "ABC-1234");
    }
}
