using Microsoft.Extensions.Logging;
using Moq;
using vehicle_rental.application.Services;
using vehicle_rental.domain.Domain.motorcycles.dtos;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.unit.tests.Application;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<ILogger<MotorcycleService>> _loggerMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _loggerMock = new Mock<ILogger<MotorcycleService>>();
        _messagePublisherMock = new Mock<IMessagePublisher>();

        _service = new MotorcycleService(
            _motorcycleRepositoryMock.Object,
            _loggerMock.Object,
            _messagePublisherMock.Object);
    }

    [Fact]
    public async Task CreateMotorcycleAsync_ValidData_ShouldCreateMotorcycleSuccessfully()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Year = "2024",
            Model = "Honda CB 600F",
            LicensePlate = "ABC-1234"
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateExactAsync(createDto.LicensePlate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        _motorcycleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle m, CancellationToken _) => m);

        // Act
        var result = await _service.CreateMotorcycleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result?.Year.Should().Be(createDto.Year);
        result?.Model.Should().Be(createDto.Model);
        result?.LicensePlate.Should().Be(createDto.LicensePlate);
        result?.Id.Should().NotBe(Guid.Empty);

        _messagePublisherMock.Verify(
            x => x.PublishMotorcycleRegisteredAsync(It.IsAny<Guid>(), createDto.Year, createDto.Model, createDto.LicensePlate),
            Times.Once);
    }

    [Fact]
    public async Task CreateMotorcycleAsync_DuplicateLicensePlate_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Year = "2024",
            Model = "Honda CB 600F",
            LicensePlate = "ABC-1234"
        };

        var existingMotorcycle = CreateTestMotorcycle();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateExactAsync(createDto.LicensePlate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMotorcycle);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateMotorcycleAsync(createDto));
    }

    [Fact]
    public async Task GetMotorcycleByIdAsync_ExistingMotorcycle_ShouldReturnMotorcycle()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var motorcycle = CreateTestMotorcycle();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        // Act
        var result = await _service.GetMotorcycleByIdAsync(motorcycleId);

        // Assert
        result.Should().NotBeNull();
        result?.Year.Should().Be(motorcycle.Year);
        result?.Model.Should().Be(motorcycle.Model);
        result?.LicensePlate.Should().Be(motorcycle.LicensePlate);
    }

    [Fact]
    public async Task GetMotorcycleByIdAsync_NonExistingMotorcycle_ShouldReturnNull()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        // Act
        var result = await _service.GetMotorcycleByIdAsync(motorcycleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMotorcyclesAsync_WithLicensePlateFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var motorcycles = new List<Motorcycle>
        {
            CreateTestMotorcycle("ABC-1234"),
            CreateTestMotorcycle("XYZ-5678")
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateAsync("ABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync([.. motorcycles.Where(m => m.LicensePlate!.Contains("ABC"))]);

        // Act
        var result = await _service.GetMotorcyclesAsync("ABC");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task GetMotorcyclesAsync_WithoutFilter_ShouldReturnAllMotorcycles()
    {
        // Arrange
        var motorcycles = new List<Motorcycle>
        {
            CreateTestMotorcycle("ABC-1234"),
            CreateTestMotorcycle("XYZ-5678")
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles);

        // Act
        var result = await _service.GetMotorcyclesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateMotorcycleLicensePlateAsync_ValidData_ShouldUpdateLicensePlate()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var motorcycle = CreateTestMotorcycle();
        var newLicensePlate = "XYZ-5678";

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateExactAsync(newLicensePlate, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        _motorcycleRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle m, CancellationToken _) => m);

        // Act
        var result = await _service.UpdateMotorcycleLicensePlateAsync(motorcycleId, newLicensePlate);

        // Assert
        result.Should().NotBeNull();
        result?.LicensePlate.Should().Be(newLicensePlate);
        motorcycle.LicensePlate.Should().Be(newLicensePlate);
    }

    [Fact]
    public async Task UpdateMotorcycleLicensePlateAsync_MotorcycleNotFound_ShouldThrowException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var newLicensePlate = "XYZ-5678";

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateMotorcycleLicensePlateAsync(motorcycleId, newLicensePlate));
    }

    [Fact]
    public async Task UpdateMotorcycleLicensePlateAsync_DuplicateLicensePlate_ShouldThrowException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var motorcycle = CreateTestMotorcycle();
        var newLicensePlate = "XYZ-5678";
        var existingMotorcycle = CreateTestMotorcycle(newLicensePlate);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateExactAsync(newLicensePlate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMotorcycle);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateMotorcycleLicensePlateAsync(motorcycleId, newLicensePlate));
    }

    [Fact]
    public async Task DeleteMotorcycleAsync_MotorcycleWithActiveRentals_ShouldThrowException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var motorcycle = CreateTestMotorcycle();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRepositoryMock
            .Setup(x => x.HasActiveRentalsAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteMotorcycleAsync(motorcycleId));
    }

    [Fact]
    public async Task DeleteMotorcycleAsync_MotorcycleWithoutActiveRentals_ShouldDeleteSuccessfully()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var motorcycle = CreateTestMotorcycle();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRepositoryMock
            .Setup(x => x.HasActiveRentalsAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _motorcycleRepositoryMock
            .Setup(x => x.DeleteAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteMotorcycleAsync(motorcycleId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateMotorcycleAsync_NullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateMotorcycleAsync(null!));
    }

    [Fact]
    public async Task UpdateMotorcycleLicensePlateAsync_NullLicensePlate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateMotorcycleLicensePlateAsync(motorcycleId, null!));
    }

    private static Motorcycle CreateTestMotorcycle(string? licensePlate = null)
    {
        return new Motorcycle("2024", "Honda CB 600F", licensePlate ?? "ABC-1234");
    }
}
