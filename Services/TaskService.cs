using daytask.Dtos;
using daytask.Models;
using daytask.Repositories;
using daytask.Exceptions;

namespace daytask.Services
{
    public class TaskService (ITaskRepository taskRepository) : ITaskService
    {
        public async Task<ApiResponse<UserTask>> CreateTaskAsync(TaskDto taskDto)
        {
            var task = new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority,
                Labels = taskDto.Labels,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = taskDto.UserId
            };

            var success = await taskRepository.CreateTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to create task");
            }

            return ApiResponse<UserTask>.SuccessResponse(task, "Task created successfully");
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> CreateTasksAsync(IEnumerable<TaskDto> tasks)
        {
            var userTasks = tasks.Select(taskDto => new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority,
                Labels = taskDto.Labels,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = taskDto.UserId
            }).ToArray();
            var success = await taskRepository.CreateTasksAsync(userTasks);
            if (!success)
            {
                throw new AppException("Failed to create tasks");
            }
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(userTasks, "Tasks created successfully");
        }

        public async Task<ApiResponse<bool>> DeleteTaskAsync(Guid id)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID {id} not found");
            }

            var success = await taskRepository.RemoveTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to delete task");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Task deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> GetAllTasksAsync()
        {
            var tasks = await taskRepository.GetAllTasksAsync();
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(tasks);
        }

        public async Task<ApiResponse<UserTask>> GetTaskByIdAsync(Guid id)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID {id} not found");
            }

            return ApiResponse<UserTask>.SuccessResponse(task);
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> GetTasksByUserIdAsync(Guid userId)
        {
            var tasks = await taskRepository.GetTasksByUserIdAsync(userId);
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(tasks);
        }

        public async Task<ApiResponse<UserTask>> UpdateTaskAsync(Guid id, TaskDto taskDto)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID {id} not found");
            }

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.DueDate = taskDto.DueDate;
            task.Priority = taskDto.Priority;
            task.Labels = taskDto.Labels;
            task.UpdatedAt = DateTime.UtcNow;

            var success = await taskRepository.UpdateTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to update task");
            }

            return ApiResponse<UserTask>.SuccessResponse(task, "Task updated successfully");
        }
    }
}
