using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using vehicle_rental.domain.Domain.motorcycleNotifications.entity;
using vehicle_rental.domain.Domain.motorcycleNotifications.interfaces;
using vehicle_rental.domain.shared.events;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.application.Messaging;

public class RabbitMQMessageConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQMessageConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string EXCHANGE_NAME = "motorcycle.events";
    private const string QUEUE_NAME = "motorcycle.notifications";
    private const string ROUTING_KEY = "motorcycle.registered";

    public RabbitMQMessageConsumer(IConfiguration configuration, ILogger<RabbitMQMessageConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

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
        _channel.QueueDeclare(QUEUE_NAME, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var motorcycleEvent = JsonSerializer.Deserialize<MotorcycleRegisteredEvent>(message);

                if (motorcycleEvent != null && motorcycleEvent.Year == "2024")
                {
                    await Process2024MotorcycleNotification(motorcycleEvent);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing motorcycle notification message");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(QUEUE_NAME, false, consumer);
        _logger.LogInformation("Started consuming motorcycle notification messages");
    }

    private async Task Process2024MotorcycleNotification(MotorcycleRegisteredEvent motorcycleEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<IMotorcycleNotificationRepository>();

        var notification = new MotorcycleNotification(motorcycleEvent.MotorcycleId,
                                                      motorcycleEvent.Year,
                                                      motorcycleEvent.Model,
                                                      motorcycleEvent.LicensePlate);

        await notificationRepository.AddAsync(notification);

        _logger.LogInformation("Stored notification for 2024 motorcycle {MotorcycleId}", motorcycleEvent.MotorcycleId);
    }

    public void StopConsuming()
    {
        _channel?.Close();
        _connection?.Close();
        _logger.LogInformation("Stopped consuming motorcycle notification messages");
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
