namespace vehicle_rental.domain.Domain.motorcycles.dtos;

public record UpdateMotorcycleLicensePlateDto
{
    public string NewLicensePlate { get; set; } = string.Empty;
}
