namespace vehicle_rental.domain.shared.events;

public class MotorcycleRegisteredEvent
{
    public Guid MotorcycleId { get; set; }
    public string? Year { get; set; }
    public string? Model { get; set; }
    public string? LicensePlate { get; set; }
    public DateTime Timestamp { get; set; }
}
