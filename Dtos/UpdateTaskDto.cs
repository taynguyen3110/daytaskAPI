namespace daytask.Dtos
{
    public class UpdateTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
        public DateTimeOffset DueDate { get; set; }
        public string Priority { get; set; } = "low";  //low, medium, high
        public string[] Labels { get; set; } = Array.Empty<string>();
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public string? Recurrence { get; set; }
        public DateTimeOffset? Reminder { get; set; }
        public DateTimeOffset? SnoozedUntil { get; set; }
        public Guid UserId { get; set; }
    }
}