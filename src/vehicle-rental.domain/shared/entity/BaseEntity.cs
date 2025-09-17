namespace vehicle_rental.domain.shared.entity;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        CreatedAt = DataHoraBrasilia();
        UpdatedAt = DataHoraBrasilia();
    }

    public static DateTime DataHoraBrasilia()
    {
        TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);
        return DateTime.SpecifyKind(brasiliaTime, DateTimeKind.Utc);
    }
}
