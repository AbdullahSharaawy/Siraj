namespace TheCharityDAL.Entities
{
    public class ScheduledJob
    {
        public int Id { get; private set; }
        public string HangfireJobId { get; private set; } = string.Empty;
        public string EntityType { get; private set; } = string.Empty;
        public int EntityId { get; private set; }
        public string JobType { get; private set; } = string.Empty;
        public DateTime ScheduledFor { get; private set; }
        public string Status { get; private set; } = "Pending";
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
    }
}
