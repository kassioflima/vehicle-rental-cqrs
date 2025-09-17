namespace vehicle_rental.domain.Domain.motorcycles.dtos;

public record MotorcycleDto
{
    public Guid Id { get; set; }
    public string Year { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}