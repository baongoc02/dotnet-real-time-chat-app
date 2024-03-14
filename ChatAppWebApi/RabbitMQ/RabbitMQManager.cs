using RabbitMQ.Client;
using System;
using System.Text;

namespace ChatAppWebApi.RabbitMQ
{
    public class RabbitMQManager : IDisposable
    {
        private readonly IModel _channel;

        public RabbitMQManager(IModel channel)
        {
            _channel = channel;
        }

        public void SendMessage(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}