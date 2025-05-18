namespace daytask.Dtos
{
    public class LoginResponseDto
    {
        public Boolean? IsAuthenticated { get; set; }
        public TokenResponseDto? Token { get; set; }
        public UserLoginDto? User { get; set; }
    }
}
