using System.Text;
using ChatAppWebApi.Commands;
using ChatAppWebApi.Hubs;
using ChatAppWebApi.RabbitMQ;
using MediatR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatAppWebApi.Controllers;

[ApiController]
[Route("[controller]")]

public class MessageController : ControllerBase
{
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly RabbitMQManager _rabbitMQManager;

    public MessageController(IHubContext<ChatHub> chatHubContext,
            RabbitMQManager rabbitMQManager)
    {
        _chatHubContext = chatHubContext;
        _rabbitMQManager = rabbitMQManager;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
    {
        // await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", messageDto.User, messageDto.Message);
        var message = messageDto.User + ": " + messageDto.Message;
        _rabbitMQManager.SendMessage("chat_queue", message);
        Console.WriteLine($" Sent {message}");

        return Ok();
    }

    [HttpGet("listen")]
    public async Task<IActionResult> ListenForMessages()
    {
        _rabbitMQManager.ListenForMessages("chat_queue", async (message) =>
        {
            Console.WriteLine($" Get {message}");
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", message);
        });

        return Ok("Listening for messages from RabbitMQ...");
    }

    [HttpGet("show-messages")]
    public async Task<IActionResult> ShowMessages()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/chatHub") // Thay đổi URL cho phù hợp với ứng dụng của bạn
            .Build();

        // Đăng ký sự kiện để nhận tin nhắn từ Hub
        connection.On<string>("ReceiveMessage", message =>
        {
            Console.WriteLine($"Received message: {message}");
            // Xử lý tin nhắn ở đây (ví dụ: lưu vào cơ sở dữ liệu, hiển thị trên giao diện người dùng, ...)
        });

        try
        {
            // Kết nối tới Hub
            await connection.StartAsync();

            Console.WriteLine("Connected to SignalR Hub.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return Ok("Listening for messages from RabbitMQ...");
    }

    public class SendMessageDto
    {
        public string User { get; set; }
        public string Message { get; set; }
    }

}
