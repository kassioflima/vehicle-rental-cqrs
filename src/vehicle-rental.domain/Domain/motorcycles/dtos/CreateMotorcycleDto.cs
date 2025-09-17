namespace vehicle_rental.domain.Domain.motorcycles.dtos;

public record CreateMotorcycleDto
{
    public string Year { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
}
