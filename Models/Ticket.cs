namespace SmartQueueAPI.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Waiting";
        public int Priority { get; set; } = 0;
    }
}
