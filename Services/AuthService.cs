using daytask.Dtos;
using daytask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using daytask.Repositories;
using daytask.Exceptions;

namespace daytask.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(UserDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user is null)
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            TokenResponseDto token = await CreateTokenResponse(user);
            
            UserLoginDto loggedInUser = new()
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
            };

            var response = new LoginResponseDto
            {
                IsAuthenticated = true,
                Token = token,
                User = loggedInUser,
                ChatId = user.ChatId?.ToString() ?? null
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
        }

        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            if (await _userRepository.CheckUserExist(request.Email))
            {
                throw new ValidationException("Email already registered");
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            var success = await _userRepository.AddUserAsync(user);
            if (!success)
            {
                throw new AppException("Failed to register user");
            }

            var response = new RegisterResponseDto
            {
                Success = true,
            };

            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Registration successful");
        }

        public async Task<ApiResponse<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            var tokenResponse = await CreateTokenResponse(user);
            return ApiResponse<TokenResponseDto>.SuccessResponse(tokenResponse, "Token refresh successful");
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            var success = await _userRepository.SaveChangesAsync();
            if (!success)
            {
                throw new AppException("Failed to save refresh token");
            }
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
