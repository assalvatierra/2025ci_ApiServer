using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;


namespace ApiServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RabbitMqController : ControllerBase
    {
        private readonly IConnection _connection;

        public RabbitMqController()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
        }


        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] string message)
        {
            message = string.IsNullOrEmpty(message) ? "Hello RabbitMQ!" : message;
            //using (var channel = ((Connection)_connection).CreateModel())
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "testQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "testQueue",
                                     basicProperties: null,
                                     body: body);
            }

            return Ok("Message sent to RabbitMQ");
        }
    }
}