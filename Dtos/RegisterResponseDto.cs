using Microsoft.Identity.Client;

namespace daytask.Dtos
{
    public class RegisterResponseDto
    {
        public required Boolean Success { get; set; } = false;
        public required string  Message { get; set; } = string.Empty;
    }
}
