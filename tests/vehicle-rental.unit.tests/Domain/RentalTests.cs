using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.unit.tests.Domain;

public class RentalTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateRentalWithCorrectProperties()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();
        var motorcycleId = Guid.NewGuid();
        var plan = ERentalPlan.SevenDays;
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(7);
        var expectedEndDate = endDate;
        var dailyRate = 30.00m;

        // Act
        var rental = new Rental(deliveryPersonId, motorcycleId, plan, startDate, endDate, expectedEndDate, dailyRate);

        // Assert
        rental.DeliveryPersonId.Should().Be(deliveryPersonId);
        rental.MotorcycleId.Should().Be(motorcycleId);
        rental.Plan.Should().Be(plan);
        rental.StartDate.Should().Be(startDate);
        rental.EndDate.Should().Be(endDate);
        rental.ExpectedEndDate.Should().Be(expectedEndDate);
        rental.DailyRate.Should().Be(dailyRate);
        rental.Status.Should().Be(RentalStatus.Active);
        rental.Id.Should().NotBe(Guid.Empty);
        rental.CreatedAt.Should().NotBe(default(DateTime)); // Should have a valid timestamp
    }

    [Fact]
    public void CalculateReturnAmount_EarlyReturn_ShouldCalculateFine()
    {
        // Arrange
        var rental = CreateTestRental(ERentalPlan.SevenDays, 30.00m);
        var earlyReturnDate = rental.StartDate.AddDays(3); // 4 days early

        // Act
        var calculation = rental.CalculateReturnAmount(earlyReturnDate);

        // Assert
        calculation.DaysUsed.Should().Be(3);
        calculation.DaysRemaining.Should().Be(4);
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().Be(24.00m); // 4 days * 30.00 * 0.20
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(114.00m); // 3 days * 30.00 + 24.00 fine
    }

    [Fact]
    public void CalculateReturnAmount_LateReturn_ShouldCalculateAdditionalDays()
    {
        // Arrange
        var rental = CreateTestRental(ERentalPlan.SevenDays, 30.00m);
        var lateReturnDate = rental.StartDate.AddDays(10); // 3 days late

        // Act
        var calculation = rental.CalculateReturnAmount(lateReturnDate);

        // Assert
        calculation.DaysUsed.Should().Be(10);
        calculation.DaysRemaining.Should().Be(0);
        calculation.AdditionalDays.Should().Be(3);
        calculation.FineAmount.Should().BeNull();
        calculation.AdditionalDaysAmount.Should().Be(150.00m); // 3 days * 50.00
        calculation.TotalAmount.Should().Be(450.00m); // 10 days * 30.00 + 150.00 additional
    }

    [Fact]
    public void CalculateReturnAmount_OnTimeReturn_ShouldNotCalculateFineOrAdditionalDays()
    {
        // Arrange
        var rental = CreateTestRental(ERentalPlan.SevenDays, 30.00m);
        var onTimeReturnDate = rental.ExpectedEndDate;

        // Act
        var calculation = rental.CalculateReturnAmount(onTimeReturnDate);

        // Assert
        calculation.DaysUsed.Should().Be(7);
        calculation.DaysRemaining.Should().Be(0);
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().BeNull();
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(210.00m); // 7 days * 30.00
    }

    [Theory]
    [InlineData(ERentalPlan.SevenDays, 0.20)]
    [InlineData(ERentalPlan.FifteenDays, 0.40)]
    [InlineData(ERentalPlan.ThirtyDays, 0.0)]
    [InlineData(ERentalPlan.FortyFiveDays, 0.0)]
    [InlineData(ERentalPlan.FiftyDays, 0.0)]
    public void CalculateReturnAmount_DifferentPlans_ShouldApplyCorrectFineRate(ERentalPlan plan, decimal expectedFineRate)
    {
        // Arrange
        var rental = CreateTestRental(plan, 30.00m);
        var earlyReturnDate = rental.StartDate.AddDays(1); // Return 1 day early

        // Act
        var calculation = rental.CalculateReturnAmount(earlyReturnDate);

        // Assert
        var expectedFine = (rental.ExpectedEndDate - earlyReturnDate).Days * rental.DailyRate * expectedFineRate;
        calculation.FineAmount.Should().Be(expectedFine);
    }

    [Fact]
    public void CompleteRental_ValidEndDate_ShouldUpdateRentalStatusAndAmounts()
    {
        // Arrange
        var rental = CreateTestRental(ERentalPlan.SevenDays, 30.00m);
        var actualEndDate = rental.StartDate.AddDays(5); // 2 days early
        var originalUpdatedAt = rental.UpdatedAt;

        // Act
        rental.CompleteRental(actualEndDate);

        // Assert
        rental.Status.Should().Be(RentalStatus.Completed);
        rental.EndDate.Should().Be(actualEndDate);
        rental.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
        rental.FineAmount.Should().Be(12.00m); // 2 days * 30.00 * 0.20
        rental.TotalAmount.Should().Be(162.00m); // 5 days * 30.00 + 12.00 fine
    }

    [Fact]
    public void CalculateReturnAmount_NegativeDaysUsed_ShouldHandleGracefully()
    {
        // Arrange
        var rental = CreateTestRental(ERentalPlan.SevenDays, 30.00m);
        var returnDateBeforeStart = rental.StartDate.AddDays(-1);

        // Act
        var calculation = rental.CalculateReturnAmount(returnDateBeforeStart);

        // Assert
        calculation.DaysUsed.Should().Be(-1);
        calculation.DaysRemaining.Should().Be(8); // 7 + 1
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().Be(48.00m); // 8 days * 30.00 * 0.20
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(18.00m); // -1 days * 30.00 + 48.00 fine
    }

    private static Rental CreateTestRental(ERentalPlan plan, decimal dailyRate)
    {
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays((int)plan);
        return new Rental(
            Guid.NewGuid(),
            Guid.NewGuid(),
            plan,
            startDate,
            endDate,
            endDate,
            dailyRate
        );
    }
}
