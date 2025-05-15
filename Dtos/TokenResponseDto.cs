using Microsoft.Identity.Client;

namespace daytask.Dtos
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string  RefreshToken { get; set; }
    }
}
