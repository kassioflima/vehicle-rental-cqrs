using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.enums;

namespace vehicle_rental.unit.tests.Domain;

public class DeliveryPersonTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateDeliveryPersonWithCorrectProperties()
    {
        // Arrange
        var name = "João Silva";
        var cnpj = "12.345.678/0001-90";
        var birthDate = new DateTime(1990, 5, 15);
        var licenseNumber = "123456789";
        var licenseType = ELicenseType.A;

        // Act
        var deliveryPerson = new DeliveryPerson(name, cnpj, birthDate, licenseNumber, licenseType);

        // Assert
        deliveryPerson.Name.Should().Be(name);
        deliveryPerson.Cnpj.Should().Be(cnpj);
        deliveryPerson.BirthDate.Should().Be(birthDate);
        deliveryPerson.LicenseNumber.Should().Be(licenseNumber);
        deliveryPerson.LicenseType.Should().Be(licenseType);
        deliveryPerson.Id.Should().NotBe(Guid.Empty);
        deliveryPerson.CreatedAt.Should().NotBe(default(DateTime)); // Should have a valid timestamp
        deliveryPerson.Rentals.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(ELicenseType.A, true)]
    [InlineData(ELicenseType.AB, true)]
    [InlineData(ELicenseType.B, false)]
    public void CanRentMotorcycle_DifferentLicenseTypes_ShouldReturnCorrectResult(ELicenseType licenseType, bool expectedResult)
    {
        // Arrange
        var deliveryPerson = CreateTestDeliveryPerson(licenseType);

        // Act
        var result = deliveryPerson.CanRentMotorcycle();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void UpdateLicenseImage_ValidImageUrl_ShouldUpdateImageUrlAndTimestamp()
    {
        // Arrange
        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);
        var imageUrl = "https://example.com/license-image.jpg";
        var originalUpdatedAt = deliveryPerson.UpdatedAt;

        // Act
        deliveryPerson.UpdateLicenseImage(imageUrl);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be(imageUrl);
        deliveryPerson.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    [Fact]
    public void UpdateLicenseImage_NullImageUrl_ShouldUpdateImageUrlToNull()
    {
        // Arrange
        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);
        deliveryPerson.UpdateLicenseImage("https://example.com/license-image.jpg");

        // Act
        deliveryPerson.UpdateLicenseImage(null!);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().BeNull();
    }

    [Fact]
    public void UpdateLicenseImage_EmptyImageUrl_ShouldUpdateImageUrlToEmpty()
    {
        // Arrange
        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);

        // Act
        deliveryPerson.UpdateLicenseImage(string.Empty);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithDifferentLicenseTypes_ShouldSetCorrectLicenseType()
    {
        // Arrange & Act
        var deliveryPersonA = CreateTestDeliveryPerson(ELicenseType.A);
        var deliveryPersonB = CreateTestDeliveryPerson(ELicenseType.B);
        var deliveryPersonAB = CreateTestDeliveryPerson(ELicenseType.AB);

        // Assert
        deliveryPersonA.LicenseType.Should().Be(ELicenseType.A);
        deliveryPersonB.LicenseType.Should().Be(ELicenseType.B);
        deliveryPersonAB.LicenseType.Should().Be(ELicenseType.AB);
    }

    [Fact]
    public void Constructor_WithSpecialCharactersInName_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var nameWithSpecialChars = "João da Silva-Santos & Cia.";
        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A, nameWithSpecialChars);

        // Assert
        deliveryPerson.Name.Should().Be(nameWithSpecialChars);
    }

    [Fact]
    public void Constructor_WithFormattedCnpj_ShouldPreserveCnpjFormat()
    {
        // Arrange
        var formattedCnpj = "12.345.678/0001-90";
        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A, cnpj: formattedCnpj);

        // Assert
        deliveryPerson.Cnpj.Should().Be(formattedCnpj);
    }

    private static DeliveryPerson CreateTestDeliveryPerson(ELicenseType licenseType, string? name = null, string? cnpj = null)
    {
        return new DeliveryPerson(
            name ?? "João Silva",
            cnpj ?? "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            licenseType
        );
    }
}
