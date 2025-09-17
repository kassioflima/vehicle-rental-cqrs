namespace vehicle_rental.domain.shared.interfaces;

public interface IMessagePublisher
{
    Task PublishMotorcycleRegisteredAsync(Guid motorcycleId, string year, string model, string licensePlate);
}

public interface IMessageConsumer
{
    void StartConsuming();
    void StopConsuming();
}
