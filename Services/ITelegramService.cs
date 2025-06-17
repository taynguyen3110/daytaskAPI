using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface ITelegramService
    {
        Task<ApiResponse<string>> AddChatIdAsync(AddChatIDRequestDto addChatIDRequestDto);
        Task<ApiResponse<string>> RemoveChatIdAsync(Guid userId);
        Task<ApiResponse<bool>> SendMessageAsync(string chatId, string message);
    }
}
