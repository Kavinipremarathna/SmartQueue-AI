using Microsoft.AspNetCore.SignalR;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Infrastructure.Realtime;

public class SignalRQueueNotifier : IQueueNotifier
{
    private readonly IHubContext<QueueHub> _hubContext;

    public SignalRQueueNotifier(IHubContext<QueueHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastQueueUpdatedAsync()
    {
        return _hubContext.Clients.All.SendAsync("queue-updated", new { utc = DateTime.UtcNow });
    }
}
