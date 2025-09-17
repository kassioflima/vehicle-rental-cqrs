using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using vehicle_rental.application.Services;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.unit.tests.Application;

public class DeliveryPersonServiceTests
{
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<ILogger<DeliveryPersonService>> _loggerMock;
    private readonly DeliveryPersonService _service;

    public DeliveryPersonServiceTests()
    {
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _loggerMock = new Mock<ILogger<DeliveryPersonService>>();

        _service = new DeliveryPersonService(_deliveryPersonRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_ValidData_ShouldCreateDeliveryPersonSuccessfully()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            Name = "João Silva",
            Cnpj = "12.345.678/0001-90",
            BirthDate = new DateTime(1990, 5, 15),
            LicenseNumber = "123456789",
            LicenseType = ELicenseType.A
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.CnpjExistsAsync(createDto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(createDto.LicenseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<DeliveryPerson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson dp, CancellationToken _) => dp);

        // Act
        var result = await _service.CreateDeliveryPersonAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Cnpj.Should().Be(createDto.Cnpj);
        result.BirthDate.Should().Be(createDto.BirthDate);
        result.LicenseNumber.Should().Be(createDto.LicenseNumber);
        result.LicenseType.Should().Be(createDto.LicenseType);
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_DuplicateCnpj_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            Name = "João Silva",
            Cnpj = "12.345.678/0001-90",
            BirthDate = new DateTime(1990, 5, 15),
            LicenseNumber = "123456789",
            LicenseType = ELicenseType.A
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.CnpjExistsAsync(createDto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeliveryPersonAsync(createDto));
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_DuplicateLicenseNumber_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            Name = "João Silva",
            Cnpj = "12.345.678/0001-90",
            BirthDate = new DateTime(1990, 5, 15),
            LicenseNumber = "123456789",
            LicenseType = ELicenseType.A
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.CnpjExistsAsync(createDto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(createDto.LicenseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeliveryPersonAsync(createDto));
    }

    [Fact]
    public async Task GetDeliveryPersonByIdAsync_ExistingDeliveryPerson_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();
        var deliveryPerson = CreateTestDeliveryPerson();

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        // Act
        var result = await _service.GetDeliveryPersonByIdAsync(deliveryPersonId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(deliveryPerson.Name);
        result.Cnpj.Should().Be(deliveryPerson.Cnpj);
    }

    [Fact]
    public async Task GetDeliveryPersonByIdAsync_NonExistingDeliveryPerson_ShouldReturnNull()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson?)null);

        // Act
        var result = await _service.GetDeliveryPersonByIdAsync(deliveryPersonId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDeliveryPersonsAsync_ShouldReturnAllDeliveryPersons()
    {
        // Arrange
        var deliveryPersons = new List<DeliveryPerson>
        {
            CreateTestDeliveryPerson(),
            CreateTestDeliveryPerson()
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPersons);

        // Act
        var result = await _service.GetDeliveryPersonsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_ExistingDeliveryPerson_ShouldUpdateLicenseImage()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();
        var deliveryPerson = CreateTestDeliveryPerson();
        var mockFormFile = CreateMockFormFile("license.png");

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        _deliveryPersonRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<DeliveryPerson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson dp, CancellationToken _) => dp);

        // Act
        var result = await _service.UpdateLicenseImageAsync(deliveryPersonId, mockFormFile.Object);

        // Assert
        result.Should().NotBeNull();
        result.LicenseImageUrl.Should().NotBeNull();
        deliveryPerson.LicenseImageUrl.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_NonExistingDeliveryPerson_ShouldThrowException()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();
        var mockFormFile = CreateMockFormFile("license.png");

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateLicenseImageAsync(deliveryPersonId, mockFormFile.Object));
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_NullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateDeliveryPersonAsync(null!));
    }

    [Theory]
    [InlineData(ELicenseType.A)]
    [InlineData(ELicenseType.AB)]
    [InlineData(ELicenseType.B)]
    public async Task CreateDeliveryPersonAsync_DifferentLicenseTypes_ShouldCreateSuccessfully(ELicenseType licenseType)
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            Name = "João Silva",
            Cnpj = "12.345.678/0001-90",
            BirthDate = new DateTime(1990, 5, 15),
            LicenseNumber = "123456789",
            LicenseType = licenseType
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.CnpjExistsAsync(createDto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(createDto.LicenseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<DeliveryPerson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson dp, CancellationToken _) => dp);

        // Act
        var result = await _service.CreateDeliveryPersonAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.LicenseType.Should().Be(licenseType);
    }

    private static DeliveryPerson CreateTestDeliveryPerson()
    {
        return new DeliveryPerson(
            "João Silva",
            "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            ELicenseType.A
        );
    }

    private static Mock<IFormFile> CreateMockFormFile(string fileName)
    {
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(x => x.FileName).Returns(fileName);
        mockFormFile.Setup(x => x.Length).Returns(1024);
        mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mockFormFile;
    }
}
