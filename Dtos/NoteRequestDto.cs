namespace daytask.Dtos
{
    public class NoteRequestDto
    {
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
    }
}
