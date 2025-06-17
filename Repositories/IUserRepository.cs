using daytask.Dtos;
using daytask.Models;

namespace daytask.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<string> GetChatIdByUserIdAsync(Guid userId);
        Task<bool> CheckUserExist(string email);
        Task<bool> AddUserAsync(User user);
        Task<bool> SaveChangesAsync();
        Task<bool> AddChatId(AddChatIDRequestDto addChatIDRequestDto);
        Task<bool> RemoveChatId(Guid userId);
    }
}
