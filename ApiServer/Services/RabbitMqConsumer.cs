using RabbitMQ.Client;
using System.Text;

namespace ApiServer.Services
{
    public class RabbitMqConsumer: BackgroundService
    {
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IConnection _connection;
        private IModel _channel;

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "testQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                _logger.LogInformation("Declared queue: testQueue. Waiting for messages...");

                var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("Received message: {message} at {time}", message, DateTimeOffset.Now);
                };

                channel.BasicConsume(queue: "testQueue",
                                     autoAck: true,
                                     consumer: consumer);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("RabbitMQ Consumer started successfully");
                }

                // Keep the service running
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);  // Reduced delay
                }

            }



        }

    }

}
