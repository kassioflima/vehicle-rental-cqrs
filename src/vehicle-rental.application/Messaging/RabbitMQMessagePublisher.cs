using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.application.Messaging;

public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQMessagePublisher> _logger;
    private const string EXCHANGE_NAME = "motorcycle.events";
    private const string ROUTING_KEY = "motorcycle.registered";

    public RabbitMQMessagePublisher(IConfiguration configuration, ILogger<RabbitMQMessagePublisher> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],
            Port = configuration.GetValue<int>("RabbitMQ:Port"),
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"],
            VirtualHost = configuration["RabbitMQ:VirtualHost"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, durable: true);
    }

    public Task PublishMotorcycleRegisteredAsync(Guid motorcycleId, string year, string model, string licensePlate)
    {
        try
        {
            var message = new
            {
                MotorcycleId = motorcycleId,
                Year = year,
                Model = model,
                LicensePlate = licensePlate,
                Timestamp = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Make message persistent
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: EXCHANGE_NAME,
                routingKey: ROUTING_KEY,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published motorcycle registered event for motorcycle {MotorcycleId}", motorcycleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing motorcycle registered event for motorcycle {MotorcycleId}", motorcycleId);
            throw;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
