using ChatAppWebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

        public async Task ListenForMessages(string queueName, Func<string, Task> messageHandler)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await messageHandler(message);
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);
        }


        public void Dispose()
        {
            _channel.Close();
        }
    }
}