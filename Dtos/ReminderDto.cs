namespace daytask.Dtos
{
    public class ReminderDto
    {
        public string ChatId { get; set; }
        public string TaskId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ReminderGroup { get; set; } = string.Empty;
        public DateTimeOffset RemindAt { get; set; }
        public string? Recurrence { get; set; }
    }
}
