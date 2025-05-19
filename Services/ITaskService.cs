using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface ITaskService
    {
        Task<ApiResponse<IEnumerable<UserTask>>> GetAllTasksAsync();
        Task<ApiResponse<UserTask>> GetTaskByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<UserTask>>> GetTasksByUserIdAsync(Guid userId);
        Task<ApiResponse<UserTask>> CreateTaskAsync(TaskDto taskDto);
        Task<ApiResponse<IEnumerable<UserTask>>> CreateTasksAsync(IEnumerable<TaskDto> tasks);
        Task<ApiResponse<UserTask>> UpdateTaskAsync(Guid id, TaskDto taskDto);
        Task<ApiResponse<bool>> DeleteTaskAsync(Guid id);
    }
}
