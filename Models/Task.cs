using System.Reflection.Emit;

namespace TaskFlow.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "low";  //low, medium, high
        public string[] Labels { get; set; } = Array.Empty<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public string Recurrence { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public string SnoozedUntil { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}