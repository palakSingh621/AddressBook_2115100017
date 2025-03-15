using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using BusinessLayer.Interface;
using Newtonsoft.Json;
using ModelLayer.DTO;

namespace Middleware.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public RabbitMQConsumer(IConfiguration config, IEmailService emailService)
        {
            _config = config;
            _emailService = emailService;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"],
                UserName = _config["RabbitMQ:Username"],
                Password = _config["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: _config["RabbitMQ:QueueName"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailMessage = JsonConvert.DeserializeObject<EmailDTO>(message);

                await _emailService.SendResetEmail(emailMessage.Email, emailMessage.Token);
            };

            channel.BasicConsume(queue: _config["RabbitMQ:QueueName"], autoAck: true, consumer: consumer);
        }
    }
}
