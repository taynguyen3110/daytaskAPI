namespace daytask.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
