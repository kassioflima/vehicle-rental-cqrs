using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.domain.Domain.rentals.entity;

public class Rental : BaseEntity
{
    public Guid DeliveryPersonId { get; private set; }
    public Guid MotorcycleId { get; private set; }
    public ERentalPlan Plan { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime ExpectedEndDate { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal? FineAmount { get; private set; }
    public decimal? AdditionalDaysAmount { get; private set; }
    public RentalStatus Status { get; private set; } = RentalStatus.Active;

    public virtual DeliveryPerson DeliveryPerson { get; private set; } = null!;
    public virtual Motorcycle Motorcycle { get; private set; } = null!;

    // Constructor for Entity Framework
    protected Rental() { }

    public Rental(Guid deliveryPersonId, Guid motorcycleId, ERentalPlan plan, DateTime startDate, DateTime endDate, DateTime expectedEndDate, decimal dailyRate)
    {
        Id = Guid.NewGuid();
        DeliveryPersonId = deliveryPersonId;
        MotorcycleId = motorcycleId;
        Plan = plan;
        StartDate = startDate;
        EndDate = endDate;
        ExpectedEndDate = expectedEndDate;
        DailyRate = dailyRate;
        TotalAmount = CalculateTotalAmount();
    }

    public Rental(Guid deliveryPersonId, Guid motorcycleId, ERentalPlan plan)
    {
        Id = Guid.NewGuid();
        DeliveryPersonId = deliveryPersonId;
        MotorcycleId = motorcycleId;
        Plan = plan;
    }

    private decimal CalculateTotalAmount()
    {
        var days = (ExpectedEndDate - StartDate).Days;
        return DailyRate * days;
    }

    public void CompleteRental(DateTime actualEndDate)
    {
        EndDate = actualEndDate;
        Status = RentalStatus.Completed;
        UpdatedAt = DataHoraBrasilia();

        // Use the same calculation logic for consistency
        var calculation = CalculateReturnAmount(actualEndDate);
        FineAmount = calculation.FineAmount;
        AdditionalDaysAmount = calculation.AdditionalDaysAmount;
        TotalAmount = calculation.TotalAmount;
    }

    public RentalCalculationDto CalculateReturnAmount(DateTime returnDate)
    {
        var expectedDays = (ExpectedEndDate - StartDate).Days;
        var actualDays = (returnDate - StartDate).Days;

        var calculation = new RentalCalculationDto
        {
            DaysUsed = actualDays,
            DaysRemaining = Math.Max(0, expectedDays - actualDays),
            AdditionalDays = Math.Max(0, actualDays - expectedDays)
        };

        if (actualDays < expectedDays)
        {
            // Early return - calculate fine
            var unusedDays = expectedDays - actualDays;
            var unusedAmount = DailyRate * unusedDays;

            calculation.FineAmount = Plan switch
            {
                ERentalPlan.SevenDays => unusedAmount * 0.20m,
                ERentalPlan.FifteenDays => unusedAmount * 0.40m,
                _ => 0m
            };
        }
        else if (actualDays > expectedDays)
        {
            // Late return - calculate additional days
            calculation.AdditionalDaysAmount = calculation.AdditionalDays * 50m;
        }

        calculation.TotalAmount = (DailyRate * actualDays) + (calculation.FineAmount ?? 0) + (calculation.AdditionalDaysAmount ?? 0);

        return calculation;
    }

    public static decimal GetDailyRate(ERentalPlan plan) => plan switch
    {
        ERentalPlan.SevenDays => 30.00m,
        ERentalPlan.FifteenDays => 28.00m,
        ERentalPlan.ThirtyDays => 22.00m,
        ERentalPlan.FortyFiveDays => 20.00m,
        ERentalPlan.FiftyDays => 18.00m,
        _ => throw new ArgumentException($"Invalid rental plan: {plan}")
    };

    public static int GetPlanDays(ERentalPlan plan) => plan switch
    {
        ERentalPlan.SevenDays => 7,
        ERentalPlan.FifteenDays => 15,
        ERentalPlan.ThirtyDays => 30,
        ERentalPlan.FortyFiveDays => 45,
        ERentalPlan.FiftyDays => 50,
        _ => throw new ArgumentException($"Invalid rental plan: {plan}")
    };

    public void SetStardDate(DateTime now)
        => StartDate = now;

    public void SetEndDate(ERentalPlan plan)
    {
        EndDate = StartDate.AddDays(GetPlanDays(plan));
        ExpectedEndDate = EndDate;
    }

    public void SetDailyRate(ERentalPlan plan)
        => DailyRate = GetDailyRate(plan);
}
