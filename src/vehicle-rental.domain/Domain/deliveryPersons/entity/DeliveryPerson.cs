using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.domain.Domain.deliveryPersons.entity;

public class DeliveryPerson : BaseEntity
{
    public string? Name { get; private set; }
    public string? Cnpj { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string? LicenseNumber { get; private set; }
    public ELicenseType LicenseType { get; private set; }
    public string? LicenseImageUrl { get; private set; }

    public virtual ICollection<Rental> Rentals { get; private set; } = new List<Rental>();

    // Constructor for Entity Framework
    protected DeliveryPerson() { }

    public DeliveryPerson(string name, string cnpj, DateTime birthDate, string licenseNumber, ELicenseType licenseType)
    {
        Id = Guid.NewGuid();
        Name = name;
        Cnpj = cnpj;
        BirthDate = birthDate;
        LicenseNumber = licenseNumber;
        LicenseType = licenseType;
    }

    public void UpdateLicenseImage(string imageUrl)
    {
        LicenseImageUrl = imageUrl;
        UpdatedAt = DataHoraBrasilia();
    }

    public bool CanRentMotorcycle()
    {
        return LicenseType == ELicenseType.A || LicenseType == ELicenseType.AB;
    }
}
