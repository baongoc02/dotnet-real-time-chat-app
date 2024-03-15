using ChatAppWebApi.RabbitMQ;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppWebApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly RabbitMQManager _rabbitMQManager;

        public ChatHub(RabbitMQManager rabbitMQManager)
        {
            _rabbitMQManager = rabbitMQManager;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            var body = user + ": " + message;
            _rabbitMQManager.SendMessage("chat_queue", message);
            Console.WriteLine($" Sent {body}");
        }
    }
}