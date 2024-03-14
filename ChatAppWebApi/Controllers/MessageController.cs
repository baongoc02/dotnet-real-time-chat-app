using ChatAppWebApi.Commands;
using ChatAppWebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppWebApi.Controllers;

[ApiController]
[Route("/api/chat-app")]

public class MessageController : ControllerBase
{

    private readonly IHubContext<ChatHub> _chatHubContext;

    public MessageController(IHubContext<ChatHub> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
    {
        await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", messageDto.User, messageDto.Message);
        return Ok();
    }

    public class SendMessageDto
    {
        public string User { get; set; }
        public string Message { get; set; }
    }

}
