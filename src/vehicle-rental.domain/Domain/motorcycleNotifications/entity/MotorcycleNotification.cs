using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.domain.Domain.motorcycleNotifications.entity;

public class MotorcycleNotification(Guid motorcycleId, string? year, string? model, string? licensePlate) : BaseEntity
{
    public Guid MotorcycleId { get; private set; } = motorcycleId;
    public string? Year { get; private set; } = year;
    public string? Model { get; private set; } = model;
    public string? LicensePlate { get; private set; } = licensePlate;
    public DateTime NotificationDate { get; private set; } = DataHoraBrasilia();
    public bool IsProcessed { get; private set; } = false;
    public DateTime? ProcessedAt { get; private set; }
}
