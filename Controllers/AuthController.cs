using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using daytask.Dtos;
using daytask.Services;
using daytask.Models;

namespace daytask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ApiResponse<RegisterResponseDto>> Register(RegisterRequestDto request)
        {
            var response = await authService.RegisterAsync(request);
            return response;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDto>> Login(UserDto request)
        {
           var result = await authService.LoginAsync(request);
            return result;
        }

        [HttpPost("refresh-token")]
        public async Task<ApiResponse<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            return result;
        }
    }
}

