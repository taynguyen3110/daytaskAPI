using daytask.Dtos;

namespace daytask.Services
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(TaskDto taskDto);
        Task<TaskResponseDto> GetTaskByIdAsync(Guid taskId);
        Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
        Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(Guid userId);
        Task<TaskResponseDto> UpdateTaskAsync(Guid taskId, TaskDto taskDto);
        Task<bool> DeleteTaskAsync(Guid taskId);
    }   
}
