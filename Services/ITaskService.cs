using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface ITaskService
    {
        Task<ApiResponse<IEnumerable<UserTask>>> GetAllTasksAsync();
        Task<ApiResponse<UserTask>> GetTaskByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<UserTask>>> GetTasksByUserIdAsync(Guid userId);
        Task<ApiResponse<UserTask>> CreateTaskAsync(CreateTaskDto taskDto);
        Task<ApiResponse<IEnumerable<UserTask>>> CreateTasksAsync(IEnumerable<CreateTaskDto> tasks);
        Task<ApiResponse<UserTask>> UpdateTaskAsync(Guid id, UpdateTaskDto taskDto);
        Task<ApiResponse<bool>> MergeTasksAsync(UserTask[] tasks);
        Task<ApiResponse<bool>> DeleteTaskAsync(Guid id);
    }
}
