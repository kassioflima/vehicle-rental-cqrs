using Microsoft.Extensions.Logging;
using Moq;
using vehicle_rental.application.Services;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.domain.Domain.rentals.interfaces;

namespace vehicle_rental.unit.tests.Application;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<ILogger<RentalService>> _loggerMock;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _loggerMock = new Mock<ILogger<RentalService>>();

        _service = new RentalService(
            _rentalRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _motorcycleRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateRentalAsync_ValidData_ShouldCreateRentalSuccessfully()
    {
        // Arrange
        var createDto = new CreateRentalDto
        {
            DeliveryPersonId = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            Plan = ERentalPlan.SevenDays
        };

        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);
        var motorcycle = CreateTestMotorcycle();

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _rentalRepositoryMock
            .Setup(x => x.IsMotorcycleAvailableAsync(createDto.MotorcycleId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _rentalRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental rental, CancellationToken _) => rental);

        // Act
        var result = await _service.CreateRentalAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result?.DeliveryPersonId.Should().Be(createDto.DeliveryPersonId);
        result?.MotorcycleId.Should().Be(createDto.MotorcycleId);
        result?.Plan.Should().Be(createDto.Plan);
        result?.Status.Should().Be(RentalStatus.Active);
        result?.DailyRate.Should().Be(30.00m); // SevenDays rate
    }

    [Fact]
    public async Task CreateRentalAsync_DeliveryPersonNotFound_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateRentalDto
        {
            DeliveryPersonId = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            Plan = ERentalPlan.SevenDays
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(createDto));
    }

    [Fact]
    public async Task CreateRentalAsync_DeliveryPersonCannotRent_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateRentalDto
        {
            DeliveryPersonId = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            Plan = ERentalPlan.SevenDays
        };

        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.B); // Cannot rent

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(createDto));
    }

    [Fact]
    public async Task CreateRentalAsync_MotorcycleNotFound_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateRentalDto
        {
            DeliveryPersonId = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            Plan = ERentalPlan.SevenDays
        };

        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(createDto));
    }

    [Fact]
    public async Task CreateRentalAsync_MotorcycleNotAvailable_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateRentalDto
        {
            DeliveryPersonId = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            Plan = ERentalPlan.SevenDays
        };

        var deliveryPerson = CreateTestDeliveryPerson(ELicenseType.A);
        var motorcycle = CreateTestMotorcycle();

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(createDto.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _rentalRepositoryMock
            .Setup(x => x.IsMotorcycleAvailableAsync(createDto.MotorcycleId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(createDto));
    }

    [Theory]
    [InlineData(ERentalPlan.SevenDays, 30.00)]
    [InlineData(ERentalPlan.FifteenDays, 28.00)]
    [InlineData(ERentalPlan.ThirtyDays, 22.00)]
    [InlineData(ERentalPlan.FortyFiveDays, 20.00)]
    [InlineData(ERentalPlan.FiftyDays, 18.00)]
    public async Task GetDailyRateAsync_DifferentPlans_ShouldReturnCorrectRates(ERentalPlan plan, decimal expectedRate)
    {
        // Act
        var result = await _service.GetDailyRateAsync(plan);

        // Assert
        result.Should().Be(expectedRate);
    }

    [Fact]
    public async Task GetDailyRateAsync_InvalidPlan_ShouldThrowException()
    {
        // Arrange
        var invalidPlan = (ERentalPlan)999;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDailyRateAsync(invalidPlan));
    }

    [Fact]
    public async Task CalculateRentalReturnAsync_ValidRental_ShouldReturnCalculation()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = CreateTestRental();
        var returnDate = rental.StartDate.AddDays(5);

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(rentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);

        // Act
        var result = await _service.CalculateRentalReturnAsync(rentalId, returnDate);

        // Assert
        result.Should().NotBeNull();
        result.DaysUsed.Should().Be(5);
        result.DaysRemaining.Should().Be(2);
        result.AdditionalDays.Should().Be(0);
        result.FineAmount.Should().Be(12.00m); // 2 days * 30.00 * 0.20
        result.TotalAmount.Should().Be(162.00m); // 5 days * 30.00 + 12.00 fine
    }

    [Fact]
    public async Task CalculateRentalReturnAsync_RentalNotFound_ShouldThrowException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var returnDate = DateTime.UtcNow;

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(rentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CalculateRentalReturnAsync(rentalId, returnDate));
    }

    [Fact]
    public async Task CompleteRentalAsync_ValidRental_ShouldCompleteRental()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = CreateTestRental();
        var returnDate = rental.StartDate.AddDays(5);

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(rentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);

        _rentalRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental r, CancellationToken _) => r);

        // Act
        var result = await _service.CompleteRentalAsync(rentalId, returnDate);

        // Assert
        result.Should().NotBeNull();
        result?.Status.Should().Be(RentalStatus.Completed);
        rental.EndDate.Should().Be(returnDate);
    }

    [Fact]
    public async Task CompleteRentalAsync_RentalNotFound_ShouldThrowException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var returnDate = DateTime.UtcNow;

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(rentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteRentalAsync(rentalId, returnDate));
    }

    [Fact]
    public async Task CompleteRentalAsync_RentalNotActive_ShouldThrowException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = CreateTestRental();
        rental.CompleteRental(rental.StartDate.AddDays(7)); // Already completed
        var returnDate = DateTime.UtcNow;

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(rentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteRentalAsync(rentalId, returnDate));
    }

    [Fact]
    public async Task CreateRentalAsync_NullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateRentalAsync(null!));
    }

    private static DeliveryPerson CreateTestDeliveryPerson(ELicenseType licenseType)
    {
        return new DeliveryPerson(
            "João Silva",
            "12.345.678/0001-90",
            new DateTime(1990, 5, 15),
            "123456789",
            licenseType
        );
    }

    private static Motorcycle CreateTestMotorcycle()
    {
        return new Motorcycle("2024", "Honda CB 600F", "ABC-1234");
    }

    private static Rental CreateTestRental()
    {
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(7);
        return new Rental(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ERentalPlan.SevenDays,
            startDate,
            endDate,
            endDate,
            30.00m
        );
    }
}
