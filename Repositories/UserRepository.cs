using Azure.Core;
using daytask.Data;
using daytask.Models;
using Microsoft.EntityFrameworkCore;

namespace daytask.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(String email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await context.Users.FindAsync(userId);
        }

        public async Task<bool> CheckUserExist(String email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
            return await SaveChangesAsync();
        }

        public async Task<bool> AddChatId(Guid userId, string chatId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.ChatId = chatId;
            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveChatId(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.ChatId = null;
            return await SaveChangesAsync();
        }
    }
}