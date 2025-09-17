using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.domain.Domain.motorcycles.entity;

public class Motorcycle : BaseEntity
{
    public string? Year { get; private set; }
    public string? Model { get; private set; }
    public string? LicensePlate { get; private set; }

    // Navigation properties
    public virtual ICollection<Rental> Rentals { get; private set; } = new List<Rental>();

    // Constructor for Entity Framework
    protected Motorcycle() { }

    public Motorcycle(string year, string model, string licensePlate)
    {
        Id = Guid.NewGuid();
        Year = year;
        Model = model;
        LicensePlate = licensePlate;
    }

    public void UpdateLicensePlate(string newLicensePlate)
    {
        LicensePlate = newLicensePlate;
        UpdatedAt = DataHoraBrasilia();
    }
}
