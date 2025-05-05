using TaskFlow.Dtos;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterRequestDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }   
}
