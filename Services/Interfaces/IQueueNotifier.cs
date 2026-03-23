namespace SmartQueueAPI.Services.Interfaces;

public interface IQueueNotifier
{
    Task BroadcastQueueUpdatedAsync();
}
