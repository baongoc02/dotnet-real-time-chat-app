using ChatAppWebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ChatAppWebApi.RabbitMQ
{
    
    public class RabbitMQWorker
    {
        private readonly IModel _rabbitMQChannel;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public RabbitMQWorker(IModel rabbitMQChannel, IHubContext<ChatHub> chatHubContext)
        {
            _rabbitMQChannel = rabbitMQChannel;
            _chatHubContext = chatHubContext;
        }
        

        public void StartListening()
        {
            _rabbitMQChannel.QueueDeclare(queue: "chat_queue",
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

            var consumer = new EventingBasicConsumer(_rabbitMQChannel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message from RabbitMQ: {0}", message);
                await _chatHubContext.Clients.All.SendAsync("ReceiveMessageFromRabbitMQ", message);
            };

            _rabbitMQChannel.BasicConsume(queue: "chat_queue",
                                          autoAck: true,
                                          consumer: consumer);

            Console.WriteLine("Listening for messages from RabbitMQ...");
        }
    }
}
