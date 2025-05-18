using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using daytask.Dtos;
using daytask.Services;

namespace daytask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {
            var response = await authService.RegisterAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(UserDto request)
        {
           var result = await authService.LoginAsync(request);
            if (result.StatusCode != 200)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.Data.AccessToken is null || result.Data.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            return Ok(result);
        }
    }
}

