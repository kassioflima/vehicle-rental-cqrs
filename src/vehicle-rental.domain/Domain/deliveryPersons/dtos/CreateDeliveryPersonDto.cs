using vehicle_rental.domain.Domain.deliveryPersons.enums;

namespace vehicle_rental.domain.Domain.deliveryPersons.dtos;

public record CreateDeliveryPersonDto
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public ELicenseType LicenseType { get; set; }
}
