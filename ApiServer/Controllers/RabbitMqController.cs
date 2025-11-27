using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Controllers
{
    public class RabbitMqMessageDto
    {
        [Required]
        public string Message { get; set; }
    }


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
        public IActionResult Send([FromBody] RabbitMqMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var message = string.IsNullOrEmpty(dto.Message) ? "Hello RabbitMQ!" : dto.Message;
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

            return Ok(new { status = "Message sent", message = dto.Message });
        }
    }
}