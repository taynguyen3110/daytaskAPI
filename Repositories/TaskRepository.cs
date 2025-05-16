using daytask.Data;
using Microsoft.EntityFrameworkCore;

namespace daytask.Repositories
{
    public class TaskRepository (AppDbContext dbContext) : ITaskRepository
    {
        public async Task CreateTaskAsync(Models.Task task)
        {
            await dbContext.Tasks.AddAsync(task);
        }
        public async Task<Models.Task?> GetTaskByIdAsync(Guid taskId)
        {
            return await dbContext.Tasks.FindAsync(taskId);
        }
        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        {
            return await dbContext.Tasks.ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetTasksByUserIdAsync(Guid userId)
        {
            return await dbContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
        public void RemoveTaskAsync(Models.Task task)
        {
            dbContext.Tasks.Remove(task);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

    }
}
