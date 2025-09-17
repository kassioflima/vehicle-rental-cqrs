namespace vehicle_rental.domain.Domain.rentals.dtos;

public record RentalCalculationDto
{
    public decimal TotalAmount { get; set; }
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalDaysAmount { get; set; }
    public int DaysUsed { get; set; }
    public int DaysRemaining { get; set; }
    public int AdditionalDays { get; set; }
}
