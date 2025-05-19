using daytask.Models;

namespace daytask.Repositories
{
    public interface ITaskRepository
    {
        Task<UserTask?> GetTaskByIdAsync(Guid id);
        Task<IEnumerable<UserTask>> GetAllTasksAsync();
        Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(Guid userId);
        Task<bool> CreateTaskAsync(UserTask task);
        Task<bool> CreateTasksAsync(IEnumerable<UserTask> tasks);
        Task<bool> UpdateTaskAsync(UserTask task);
        Task<bool> RemoveTaskAsync(UserTask task);
        Task<bool> SaveChangesAsync();
    }
}
