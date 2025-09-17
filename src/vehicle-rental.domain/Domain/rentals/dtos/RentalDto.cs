using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.domain.Domain.rentals.dtos;

public record RentalDto
{
    public Guid Id { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public Guid MotorcycleId { get; set; }
    public ERentalPlan Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalDaysAmount { get; set; }
    public RentalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
