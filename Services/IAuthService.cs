using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(UserDto request);
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request);
    }   
}
