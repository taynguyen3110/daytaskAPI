namespace daytask.Repositories
{
    public interface ITaskRepository
    {
        Task CreateTaskAsync(Models.Task task);
        Task<Models.Task?> GetTaskByIdAsync(Guid taskId);
        Task<IEnumerable<Models.Task>> GetAllTasksAsync();
        Task<IEnumerable<Models.Task>> GetTasksByUserIdAsync(Guid userId);
        void RemoveTaskAsync(Models.Task task);
        Task SaveChangesAsync();
    }
}
