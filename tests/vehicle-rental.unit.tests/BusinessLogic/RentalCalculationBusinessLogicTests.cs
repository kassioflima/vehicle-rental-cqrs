using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.unit.tests.BusinessLogic;

public class RentalCalculationBusinessLogicTests
{
    [Theory]
    [InlineData(ERentalPlan.SevenDays, 1, 1, 0, 6.00)] // 1 day early, 20% fine
    [InlineData(ERentalPlan.SevenDays, 2, 2, 0, 12.00)] // 2 days early, 20% fine
    [InlineData(ERentalPlan.FifteenDays, 3, 3, 0, 36.00)] // 3 days early, 40% fine
    [InlineData(ERentalPlan.FifteenDays, 5, 5, 0, 60.00)] // 5 days early, 40% fine
    public void CalculateReturnAmount_EarlyReturn_ShouldCalculateCorrectFine(
        ERentalPlan plan, int daysEarly, int expectedDaysRemaining, int expectedAdditionalDays, decimal expectedFine)
    {
        // Arrange
        var rental = CreateRentalWithPlan(plan, 30.00m);
        var returnDate = rental.StartDate.AddDays((int)plan - daysEarly);

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysRemaining.Should().Be(expectedDaysRemaining);
        calculation.AdditionalDays.Should().Be(expectedAdditionalDays);
        calculation.FineAmount.Should().Be(expectedFine);
        calculation.AdditionalDaysAmount.Should().BeNull();
    }

    [Theory]
    [InlineData(ERentalPlan.SevenDays, 1, 0, 1, 50.00)] // 1 day late, 50 per day
    [InlineData(ERentalPlan.SevenDays, 3, 0, 3, 150.00)] // 3 days late, 50 per day
    [InlineData(ERentalPlan.FifteenDays, 2, 0, 2, 100.00)] // 2 days late, 50 per day
    [InlineData(ERentalPlan.FifteenDays, 5, 0, 5, 250.00)] // 5 days late, 50 per day
    public void CalculateReturnAmount_LateReturn_ShouldCalculateCorrectAdditionalDays(
        ERentalPlan plan, int daysLate, int expectedDaysRemaining, int expectedAdditionalDays, decimal expectedAdditionalAmount)
    {
        // Arrange
        var rental = CreateRentalWithPlan(plan, 30.00m);
        var returnDate = rental.StartDate.AddDays((int)plan + daysLate);

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysRemaining.Should().Be(expectedDaysRemaining);
        calculation.AdditionalDays.Should().Be(expectedAdditionalDays);
        calculation.FineAmount.Should().BeNull();
        calculation.AdditionalDaysAmount.Should().Be(expectedAdditionalAmount);
    }

    [Theory]
    [InlineData(ERentalPlan.SevenDays, 0, 0)] // On time
    [InlineData(ERentalPlan.FifteenDays, 0, 0)] // On time
    [InlineData(ERentalPlan.ThirtyDays, 0, 0)] // On time
    public void CalculateReturnAmount_OnTimeReturn_ShouldNotCalculateFineOrAdditionalDays(
        ERentalPlan plan, int expectedDaysRemaining, int expectedAdditionalDays)
    {
        // Arrange
        var rental = CreateRentalWithPlan(plan, 30.00m);
        var returnDate = rental.ExpectedEndDate;

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysRemaining.Should().Be(expectedDaysRemaining);
        calculation.AdditionalDays.Should().Be(expectedAdditionalDays);
        calculation.FineAmount.Should().BeNull();
        calculation.AdditionalDaysAmount.Should().BeNull();
    }

    [Fact]
    public void CalculateReturnAmount_EdgeCase_ReturnBeforeStartDate_ShouldHandleGracefully()
    {
        // Arrange
        var rental = CreateRentalWithPlan(ERentalPlan.SevenDays, 30.00m);
        var returnDate = rental.StartDate.AddDays(-1);

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysUsed.Should().Be(-1);
        calculation.DaysRemaining.Should().Be(8); // 7 + 1
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().Be(48.00m); // 8 days * 30.00 * 0.20
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(18.00m); // -1 days * 30.00 + 48.00 fine
    }

    [Fact]
    public void CalculateReturnAmount_EdgeCase_ReturnExactlyOnStartDate_ShouldCalculateCorrectly()
    {
        // Arrange
        var rental = CreateRentalWithPlan(ERentalPlan.SevenDays, 30.00m);
        var returnDate = rental.StartDate;

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysUsed.Should().Be(0);
        calculation.DaysRemaining.Should().Be(7);
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().Be(42.00m); // 7 days * 30.00 * 0.20
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(42.00m); // Only fine
    }

    [Theory]
    [InlineData(ERentalPlan.SevenDays, 30.00, 210.00)] // 7 days * 30.00
    [InlineData(ERentalPlan.FifteenDays, 28.00, 420.00)] // 15 days * 28.00
    [InlineData(ERentalPlan.ThirtyDays, 22.00, 660.00)] // 30 days * 22.00
    public void CalculateReturnAmount_OnTimeReturn_ShouldCalculateCorrectTotalAmount(
        ERentalPlan plan, decimal dailyRate, decimal expectedTotal)
    {
        // Arrange
        var rental = CreateRentalWithPlan(plan, dailyRate);
        var returnDate = rental.ExpectedEndDate;

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public void CalculateReturnAmount_ComplexScenario_EarlyReturnWithDifferentRates_ShouldCalculateCorrectly()
    {
        // Arrange
        var rental = CreateRentalWithPlan(ERentalPlan.FifteenDays, 28.00m);
        var returnDate = rental.StartDate.AddDays(10); // 5 days early

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysUsed.Should().Be(10);
        calculation.DaysRemaining.Should().Be(5);
        calculation.AdditionalDays.Should().Be(0);
        calculation.FineAmount.Should().Be(56.00m); // 5 days * 28.00 * 0.40
        calculation.AdditionalDaysAmount.Should().BeNull();
        calculation.TotalAmount.Should().Be(336.00m); // 10 days * 28.00 + 56.00 fine
    }

    [Fact]
    public void CalculateReturnAmount_ComplexScenario_LateReturnWithDifferentRates_ShouldCalculateCorrectly()
    {
        // Arrange
        var rental = CreateRentalWithPlan(ERentalPlan.ThirtyDays, 22.00m);
        var returnDate = rental.StartDate.AddDays(35); // 5 days late

        // Act
        var calculation = rental.CalculateReturnAmount(returnDate);

        // Assert
        calculation.DaysUsed.Should().Be(35);
        calculation.DaysRemaining.Should().Be(0);
        calculation.AdditionalDays.Should().Be(5);
        calculation.FineAmount.Should().BeNull();
        calculation.AdditionalDaysAmount.Should().Be(250.00m); // 5 days * 50.00
        calculation.TotalAmount.Should().Be(1020.00m); // 35 days * 22.00 + 250.00 additional
    }

    [Fact]
    public void CompleteRental_ShouldUpdateAllPropertiesCorrectly()
    {
        // Arrange
        var rental = CreateRentalWithPlan(ERentalPlan.SevenDays, 30.00m);
        var actualEndDate = rental.StartDate.AddDays(5); // 2 days early
        var originalUpdatedAt = rental.UpdatedAt;

        // Act
        rental.CompleteRental(actualEndDate);

        // Assert
        rental.Status.Should().Be(RentalStatus.Completed);
        rental.EndDate.Should().Be(actualEndDate);
        rental.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
        rental.FineAmount.Should().Be(12.00m); // 2 days * 30.00 * 0.20
        rental.AdditionalDaysAmount.Should().BeNull();
        rental.TotalAmount.Should().Be(162.00m); // 5 days * 30.00 + 12.00 fine
    }

    private static Rental CreateRentalWithPlan(ERentalPlan plan, decimal dailyRate)
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
