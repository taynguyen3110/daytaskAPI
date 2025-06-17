namespace daytask.Dtos
{
    public class AddChatIDRequestDto
    {
        public Guid UserId { get; set; }
        public string ChatId { get; set; }
    }
}
