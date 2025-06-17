using daytask.Data;
using daytask.Dtos;
using daytask.Models;
using Microsoft.EntityFrameworkCore;

namespace daytask.Repositories
{
    public class TaskRepository (AppDbContext dbContext) : ITaskRepository
    {
        public async Task<UserTask?> GetTaskByIdAsync(Guid id)
        {
            return await dbContext.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<UserTask>> GetAllTasksAsync()
        {
            return await dbContext.Tasks.ToListAsync();
        }

        public async Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(Guid userId)
        {
            return await dbContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<UserTask>> GetExistingTasksByIdsAsync(IEnumerable<Guid> taskIds)
        {
            return await dbContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToListAsync();
        }
        public async Task<bool> CreateTaskAsync(UserTask task)
        {
            await dbContext.Tasks.AddAsync(task);
            return await SaveChangesAsync();
        }

        public async Task<bool> CreateTasksAsync(IEnumerable<UserTask> tasks)
        {
            await dbContext.Tasks.AddRangeAsync(tasks);
            return await SaveChangesAsync();
        }
        public async Task<bool> UpdateTaskAsync(UserTask task)
        {
            dbContext.Tasks.Update(task);
            return await SaveChangesAsync();
        }
        public async Task<bool> UpdateTasksAsync(IEnumerable<UserTask> tasks)
        {
            dbContext.Tasks.UpdateRange(tasks);
            return await SaveChangesAsync();
        }
        public async Task<bool> RemoveTaskAsync(UserTask task)
        {
            dbContext.Tasks.Remove(task);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
