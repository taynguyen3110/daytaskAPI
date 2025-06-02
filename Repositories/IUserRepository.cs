using daytask.Models;

namespace daytask.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CheckUserExist(string email);
        Task<bool> AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<bool> SaveChangesAsync();
        Task<bool> AddChatId(Guid userId, string chatId);
        Task<bool> RemoveChatId(Guid userId);
    }
}
