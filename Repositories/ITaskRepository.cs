using daytask.Models;

namespace daytask.Repositories
{
    public interface ITaskRepository
    {
        Task<UserTask?> GetTaskByIdAsync(Guid id);
        Task<IEnumerable<UserTask>> GetAllTasksAsync();
        Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(Guid userId);
        Task<IEnumerable<UserTask>> GetExistingTasksByIdsAsync(IEnumerable<Guid> taskIds);
        Task<bool> CreateTaskAsync(UserTask task);
        Task<bool> CreateTasksAsync(IEnumerable<UserTask> tasks);
        Task<bool> UpdateTaskAsync(UserTask task);
        Task<bool> UpdateTasksAsync(IEnumerable<UserTask> tasks);
        Task<bool> RemoveTaskAsync(UserTask task);
        Task<bool> SaveChangesAsync();
    }
}
