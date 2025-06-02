using daytask.Exceptions;
using daytask.Models;
using daytask.Repositories;

namespace daytask.Services
{
    public class TelegramService (IUserRepository userRepository) : ITelegramService
    {
        public async Task<ApiResponse<string>> AddChatIdAsync(Guid userId, string chatId)
        {
            var success = await userRepository.AddChatId(userId, chatId);
            if (!success)
            {
                throw new AppException("Failed to add chat ID");
            }
            return ApiResponse<string>.SuccessResponse(chatId, "Chat ID added successfully");
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
    }
}
