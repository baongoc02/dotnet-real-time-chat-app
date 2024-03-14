using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppWebApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IModel _rabbitMQChannel;

    public ChatHub(IModel rabbitMQChannel)
    {
        _rabbitMQChannel = rabbitMQChannel;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
        
        var body = Encoding.UTF8.GetBytes($"[{user}]: {message}");
        _rabbitMQChannel.BasicPublish(exchange: "",
                                      routingKey: "chat_queue",
                                      basicProperties: null,
                                      body: body);
    }
    }
}