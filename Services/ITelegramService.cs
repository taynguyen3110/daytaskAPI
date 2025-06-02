using daytask.Models;

namespace daytask.Services
{
    public interface ITelegramService
    {
        Task<ApiResponse<string>> AddChatIdAsync(Guid userId, string chatId);
        Task<ApiResponse<string>> RemoveChatIdAsync(Guid userId);
    }
}
