using Azure.Core;
using daytask.Data;
using daytask.Models;
using Microsoft.EntityFrameworkCore;

namespace daytask.Repositories
{
    public class UserRepository (AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(String email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await context.Users.FindAsync(userId);
        }

        public async Task<Boolean> CheckUserExist(String email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<Boolean> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<Boolean> AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
            return await SaveChangesAsync();
        }


    }
}
