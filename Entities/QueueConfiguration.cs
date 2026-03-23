namespace SmartQueueAPI.Entities;

public class QueueConfiguration
{
    public int Id { get; set; }
    public int StaffCount { get; set; } = 2;
    public int AverageServiceMinutes { get; set; } = 6;
}
