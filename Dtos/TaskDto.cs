﻿namespace daytask.Dtos
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset DueDate { get; set; }
        public string Priority { get; set; } = "low";  //low, medium, high
        public string[] Labels { get; set; } = Array.Empty<string>();
        public Guid UserId { get; set; }
    }
}