using daytask.Dtos;
using daytask.Exceptions;
using daytask.Models;
using daytask.Options;
using daytask.Repositories;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;

namespace daytask.Services
{
    public class TelegramService (IUserRepository userRepository, HttpClient httpClient, IOptions<TelegramBotOptions> options) : ITelegramService
    {
        public async Task<ApiResponse<string>> AddChatIdAsync(AddChatIDRequestDto addChatIDRequestDto)
        {
            var success = await userRepository.AddChatId(addChatIDRequestDto);
            if (!success)
            {
                throw new AppException("Failed to add chat ID");
            }
            return ApiResponse<string>.SuccessResponse(addChatIDRequestDto.ChatId, "Chat ID added successfully");
        }
        public async Task<ApiResponse<string>> RemoveChatIdAsync(Guid userId)
        {
            var success = await userRepository.RemoveChatId(userId);
            if (!success)
            {
                throw new AppException("Failed to remove chat ID");
            }
            return ApiResponse<string>.SuccessResponse(string.Empty, "Chat ID removed successfully");
        }

        public async Task<ApiResponse<bool>> SendMessageAsync(string chatId, string message)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{options.Value.Token}/sendMessage";
                var content = new StringContent(
                    $"{{\"chat_id\":\"{chatId}\",\"text\":\"{EscapeText(message)}\"}}",
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(url, content);
                return ApiResponse<bool>.SuccessResponse(response.IsSuccessStatusCode, "Telegram message sent successfully");
            }
            catch (Exception ex)
            {
                throw new AppException($"Failed to send Telegram message: {ex.Message}");
            }
        }
        private string EscapeText(string input)
        {
            return input.Replace("\"", "\\\""); // Basic escape
        }
    }
}
