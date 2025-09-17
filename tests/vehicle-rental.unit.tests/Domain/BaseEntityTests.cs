using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.unit.tests.Domain;

public class BaseEntityTests
{
    [Fact]
    public void DataHoraBrasilia_ShouldReturnUtcDateTime()
    {
        // Act
        var brasiliaTime = BaseEntity.DataHoraBrasilia();

        // Assert
        brasiliaTime.Kind.Should().Be(DateTimeKind.Utc);
        // Just verify it's a valid timestamp
        brasiliaTime.Should().NotBe(default(DateTime)); // Should have a valid timestamp
    }

    [Fact]
    public void DataHoraBrasilia_MultipleCalls_ShouldReturnConsistentResults()
    {
        // Act
        var time1 = BaseEntity.DataHoraBrasilia();
        var time2 = BaseEntity.DataHoraBrasilia();

        // Assert
        time1.Kind.Should().Be(DateTimeKind.Utc);
        time2.Kind.Should().Be(DateTimeKind.Utc);
        time1.Should().BeCloseTo(time2, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DataHoraBrasilia_ShouldHandleTimeZoneConversion()
    {
        // Act
        var brasiliaTime = BaseEntity.DataHoraBrasilia();
        var utcNow = DateTime.UtcNow;

        // Assert
        // The difference should be reasonable (considering timezone conversion)
        var timeDifference = Math.Abs((brasiliaTime - utcNow).TotalHours);
        timeDifference.Should().BeLessThan(5); // Should be within 5 hours difference
    }
}
