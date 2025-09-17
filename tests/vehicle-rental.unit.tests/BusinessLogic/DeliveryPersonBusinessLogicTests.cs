using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.enums;

namespace vehicle_rental.unit.tests.BusinessLogic;

public class DeliveryPersonBusinessLogicTests
{
    [Theory]
    [InlineData(ELicenseType.A, true)]
    [InlineData(ELicenseType.AB, true)]
    [InlineData(ELicenseType.B, false)]
    public void CanRentMotorcycle_DifferentLicenseTypes_ShouldReturnCorrectResult(ELicenseType licenseType, bool expectedResult)
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(licenseType);

        // Act
        var result = deliveryPerson.CanRentMotorcycle();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void UpdateLicenseImage_ShouldUpdateImageUrlAndTimestamp()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A);
        var imageUrl = "https://example.com/license-image.jpg";
        var originalUpdatedAt = deliveryPerson.UpdatedAt;

        // Act
        deliveryPerson.UpdateLicenseImage(imageUrl);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be(imageUrl);
        deliveryPerson.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    [Fact]
    public void UpdateLicenseImage_MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A);
        var firstImageUrl = "https://example.com/license-image-1.jpg";
        var secondImageUrl = "https://example.com/license-image-2.jpg";

        // Act
        deliveryPerson.UpdateLicenseImage(firstImageUrl);
        var firstUpdateTime = deliveryPerson.UpdatedAt;

        deliveryPerson.UpdateLicenseImage(secondImageUrl);
        var secondUpdateTime = deliveryPerson.UpdatedAt;

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be(secondImageUrl);
        secondUpdateTime.Should().BeAfter(firstUpdateTime!.Value);
    }

    [Fact]
    public void UpdateLicenseImage_NullImageUrl_ShouldUpdateToNull()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A);
        deliveryPerson.UpdateLicenseImage("https://example.com/license-image.jpg");

        // Act
        deliveryPerson.UpdateLicenseImage(null!);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().BeNull();
    }

    [Fact]
    public void UpdateLicenseImage_EmptyImageUrl_ShouldUpdateToEmpty()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A);

        // Act
        deliveryPerson.UpdateLicenseImage(string.Empty);

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var name = "João da Silva Santos";
        var cnpj = "12.345.678/0001-90";
        var birthDate = new DateTime(1990, 5, 15);
        var licenseNumber = "123456789";
        var licenseType = ELicenseType.AB;

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

    [Fact]
    public void Constructor_WithSpecialCharactersInName_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var nameWithSpecialChars = "João da Silva-Santos & Cia. Ltda.";
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A, nameWithSpecialChars);

        // Assert
        deliveryPerson.Name.Should().Be(nameWithSpecialChars);
    }

    [Fact]
    public void Constructor_WithFormattedCnpj_ShouldPreserveCnpjFormat()
    {
        // Arrange
        var formattedCnpj = "12.345.678/0001-90";
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A, cnpj: formattedCnpj);

        // Assert
        deliveryPerson.Cnpj.Should().Be(formattedCnpj);
    }

    [Fact]
    public void Constructor_WithLongLicenseNumber_ShouldPreserveLicenseNumber()
    {
        // Arrange
        var longLicenseNumber = "12345678901234567890";
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A, licenseNumber: longLicenseNumber);

        // Assert
        deliveryPerson.LicenseNumber.Should().Be(longLicenseNumber);
    }

    [Fact]
    public void CanRentMotorcycle_LicenseTypeA_ShouldReturnTrue()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.A);

        // Act
        var result = deliveryPerson.CanRentMotorcycle();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanRentMotorcycle_LicenseTypeAB_ShouldReturnTrue()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.AB);

        // Act
        var result = deliveryPerson.CanRentMotorcycle();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanRentMotorcycle_LicenseTypeB_ShouldReturnFalse()
    {
        // Arrange
        var deliveryPerson = CreateDeliveryPersonWithLicense(ELicenseType.B);

        // Act
        var result = deliveryPerson.CanRentMotorcycle();

        // Assert
        result.Should().BeFalse();
    }

    private static DeliveryPerson CreateDeliveryPersonWithLicense(
        ELicenseType licenseType,
        string? name = null,
        string? cnpj = null,
        string? licenseNumber = null)
    {
        return new DeliveryPerson(
            name ?? "João Silva",
            cnpj ?? "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            licenseNumber ?? "123456789",
            licenseType
        );
    }
}
