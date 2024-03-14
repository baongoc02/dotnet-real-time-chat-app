using ChatAppWebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppWebApi.Commands;

public class SendMessageCommand : IRequest
{
  public string User { get; set; }

  public string Message { get; set; }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageCommandHandler(IHubContext<ChatHub> hubContext)
    {
      _hubContext = hubContext;
    }

    public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
      await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);
      return;
    }
}
